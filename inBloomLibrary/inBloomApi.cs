using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using OAuthLibrary;
using inBloomLibrary.Models;

namespace inBloomLibrary
{
    public static class inBloomApi
    {
        public static string CreateGradebookEntry(string sectionId, Assignment assignment)
        {
            const string gradebookEntryEndpoint =
                "https://api.sandbox.inbloom.org/api/rest/v1.1/gradebookEntries";
            const string gradebookEntryCustomEndpoint =
                "https://api.sandbox.inbloom.org/api/rest/v1.1/gradebookEntries/{0}/custom";

            var account = inBloomSandboxClient.GetCurrentInBloomAccount();
            // Educators are not allowed to create gradebook entries in the SLC,
            // so use a long-lived session token of an administrator
            //var token = OAuthTokens.GetAccessToken(account.Provider, account.ProviderUserId);
            var token = GetLongLivedSessionToken(account.TenantId);

            string gradebookEntryId = null;
            var gradebookEntry = new GradebookEntry
            {
                DateAssigned = DateTime.Today,
                Description = assignment.Description,
                GradebookEntryType = string.Format("{0}-{1}", "Assignment", assignment.AssignmentId),
                SectionId = sectionId
            };

            var request = CreateWebRequest(gradebookEntryEndpoint, token);
            request.Method = "POST";
            try
            {
                using (var requestStream = request.GetRequestStream())
                {
                    JsonHelper.Serialize(requestStream, gradebookEntry);
                }
                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    if (response != null)
                    {
                        var location = response.Headers["Location"];
                        var index = location.LastIndexOf('/');
                        gradebookEntryId = location.Substring(index + 1);
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }

            var gradebookEntryCustom = new GradebookEntryCustom
            {
                ConsumerKey = assignment.ConsumerKey,
                ConsumerSecret = assignment.ConsumerSecret,
                CustomParameters = assignment.CustomParameters,
                Name = assignment.Name,
                Url = assignment.Url
            };

            request = CreateWebRequest(string.Format(gradebookEntryCustomEndpoint, gradebookEntryId), token);
            request.Method = "POST";
            try
            {
                using (var requestStream = request.GetRequestStream())
                {
                    JsonHelper.Serialize(requestStream, gradebookEntryCustom);
                }
                using (request.GetResponse())
                {
                }
            }
            catch (Exception)
            {
                return null;
            }


            return gradebookEntryId;
        }

        public static string CreateStudentGradebookEntry(string sectionId, AssignmentScore score, string token)
        {
            const string studentGradebookEntriesEndpoint =
                "https://api.sandbox.inbloom.org/api/rest/v1.1/studentGradebookEntries";

            var associations = GetStudentSectionAssociations(sectionId, token);
            var association = associations.FirstOrDefault(a => a.StudentId == score.StudentId);

            var studentGradebookEntry = new StudentGradebookEntry
            {
                DateFulfilled = DateTime.Now,
                GradebookEntryId = score.GradebookEntryId,
                NumericGradeEarned = score.NumericGradeEarned,
                SectionId = sectionId,
                StudentId = score.StudentId,
                StudentSectionAssociationId = association == null ? null : association.Id
            };

            var request = CreateWebRequest(studentGradebookEntriesEndpoint, token);
            request.Method = "POST";
            try
            {
                using (var requestStream = request.GetRequestStream())
                {
                    JsonHelper.Serialize(requestStream, studentGradebookEntry);
                }
                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    if (response != null)
                    {
                        var location = response.Headers["Location"];
                        var index = location.LastIndexOf('/');
                        return location.Substring(index + 1);
                    }
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        internal static HttpWebRequest CreateWebRequest(string url)
        {
            var request = WebRequest.Create(url) as HttpWebRequest;
            if (request != null)
            {
                request.Accept = "application/vnd.slc+json";
                request.ContentType = "application/vnd.slc+json";
                return request;
            }
            return null;
        }

        internal static HttpWebRequest CreateWebRequest(string url, string accessToken)
        {
            var request = CreateWebRequest(url);
            var bearerToken = string.Format("bearer {0}", MessagingUtilities.EscapeUriDataStringRfc3986(accessToken));
            request.Headers.Add("Authorization", bearerToken);
            return request;
        }

        public static void DeleteGradebookEntry(string gradebookEntryId)
        {
            const string gradebookEntryEndpoint =
                "https://api.sandbox.inbloom.org/api/rest/v1.1/gradebookEntries/{0}";

            var account = inBloomSandboxClient.GetCurrentInBloomAccount();
            // Educators are not allowed to delete gradebook entries in the SLC,
            // so use a long-lived session token of an administrator
            //var token = OAuthTokens.GetAccessToken(account.Provider, account.ProviderUserId);
            var token = GetLongLivedSessionToken(account.TenantId);

            var request = CreateWebRequest(string.Format(gradebookEntryEndpoint, gradebookEntryId), token);
            request.Method = "DELETE";

            try
            {
                using (request.GetResponse() as HttpWebResponse)
                {
                }
            }
// ReSharper disable EmptyGeneralCatchClause
            catch (Exception)
// ReSharper restore EmptyGeneralCatchClause
            {
            }
        }

        private static GradebookEntryCustom GetGradebookEntryCustom(string gradebookEntryId, string token)
        {
            const string gradebookEntryCustomEndpoint =
                "https://api.sandbox.inbloom.org/api/rest/v1.1/gradebookEntries/{0}/custom";

            var request = CreateWebRequest(string.Format(gradebookEntryCustomEndpoint, gradebookEntryId), token);

            try
            {
                using (var response = request.GetResponse())
                {
                    using (var responseStream = response.GetResponseStream())
                    {
                        return JsonHelper.Deserialize<GradebookEntryCustom>(responseStream);
                    }
                }
            }
            catch (Exception ex)
            {
                var webEx = ex as WebException;
                if (webEx != null)
                {
                    var response = webEx.Response as HttpWebResponse;
                    if (response != null)
                    {
                        if (response.StatusCode == HttpStatusCode.NotFound)
                        {
                            return null;
                        }
                    }
                }
                return null;
            }
        }

        private static string GetLongLivedSessionToken(string tenantId)
        {
            string token = null;
            using (var db = new inBloomLibraryContext())
            {
                var tenant = db.Tenants.SingleOrDefault(t => t.inBloomTenantId == tenantId);
                if (tenant != null) token = tenant.LongLivedSessionToken;
            }
            return token;
        }

        /// <summary>
        /// Get Section details, including GradebookEntries.
        /// </summary>
        /// <param name="sectionId"></param>
        /// <returns>Section with GradebookEntries filled in.</returns>
        public static Section GetSection(string sectionId)
        {
            const string sectionEndpoint =
                "https://api.sandbox.inbloom.org/api/rest/v1.1/sections/{0}";

            var account = inBloomSandboxClient.GetCurrentInBloomAccount();
            var token = OAuthTokens.GetAccessToken(account.Provider, account.ProviderUserId);
            var request = CreateWebRequest(string.Format(sectionEndpoint, sectionId), token);

            Section section;
            try
            {
                using (var response = request.GetResponse())
                {
                    using (var responseStream = response.GetResponseStream())
                    {
                        section = JsonHelper.Deserialize<Section>(responseStream);
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }

            section.GradebookEntries = GetSectionGradebookEntries(section.Id, token);
            section.Students = GetSectionStudents(section.Id, token);
            return section;
        }

        private static IEnumerable<GradebookEntry> GetSectionGradebookEntries(string sectionId, string token)
        {
            const string gradebookEntriesEndpoint =
                "https://api.sandbox.inbloom.org/api/rest/v1.1/gradebookEntries?sectionId={0}";

            IEnumerable<GradebookEntry> entries;

            var request = CreateWebRequest(string.Format(gradebookEntriesEndpoint, sectionId), token);
            try
            {
                using (var response = request.GetResponse())
                {
                    using (var responseStream = response.GetResponseStream())
                    {
                        entries = JsonHelper.Deserialize<GradebookEntry[]>(responseStream);
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }

            foreach (var entry in entries)
            {
                entry.Custom = GetGradebookEntryCustom(entry.Id, token);
            }

            return entries;
        }

        public static IEnumerable<Section> GetSectionsForTeacher()
        {
            const string teacherSectionsEndpoint =
                "https://api.sandbox.inbloom.org/api/rest/v1.1/teachers/{0}/teacherSectionAssociations/sections";

            var account = inBloomSandboxClient.GetCurrentInBloomAccount();
            var token = OAuthTokens.GetAccessToken(account.Provider, account.ProviderUserId);
            var request = CreateWebRequest(string.Format(teacherSectionsEndpoint, account.SlcUserId), token);

            try
            {
                using (var response = request.GetResponse())
                {
                    using (var responseStream = response.GetResponseStream())
                    {
                        return JsonHelper.Deserialize<Section[]>(responseStream);
                    }
                }
            }
            catch (Exception)
            {
                return new Section[] { };
            }
        }

        private static IEnumerable<Student> GetSectionStudents(string sectionId, string token)
        {
            const string sectionStudentsEndpoint =
                "https://api.sandbox.inbloom.org/api/rest/v1.1/sections/{0}/studentSectionAssociations/students";

            var request = CreateWebRequest(string.Format(sectionStudentsEndpoint, sectionId), token);

            try
            {
                using (var response = request.GetResponse())
                {
                    using (var responseStream = response.GetResponseStream())
                    {
                        return JsonHelper.Deserialize<Student[]>(responseStream);
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static IEnumerable<StudentSectionAssociation> GetStudentSectionAssociations(string sectionId, string token)
        {
            const string studentSectionAssociationsEndpoint =
                "https://api.sandbox.inbloom.org/api/rest/v1.1/sections/{0}/studentSectionAssociations";

            var request = CreateWebRequest(string.Format(studentSectionAssociationsEndpoint, sectionId), token);

            try
            {
                using (var response = request.GetResponse())
                {
                    using (var responseStream = response.GetResponseStream())
                    {
                        return JsonHelper.Deserialize<StudentSectionAssociation[]>(responseStream);
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static void SetStudentGradebookEntryScore(AssignmentScore score)
        {
            const string gradebookEntriesEndpoint =
                "https://api.sandbox.inbloom.org/api/rest/v1.1/gradebookEntries/{0}";
            const string studentGradebookEntriesEndpoint =
                "https://api.sandbox.inbloom.org/api/rest/v1.1/gradebookEntries/{0}/studentGradebookEntries";

            // This may be called at any time, even when no one is logged in.
            var token = GetLongLivedSessionToken(score.TenantId);

            string sectionId = null;
            var request = CreateWebRequest(string.Format(gradebookEntriesEndpoint, score.GradebookEntryId), token);
            try
            {
                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    if (response != null)
                    {
                        using (var responseStream = response.GetResponseStream())
                        {
                            var gradebookEntry = JsonHelper.Deserialize<GradebookEntry>(responseStream);
                            sectionId = gradebookEntry.SectionId;
                        }
                    }
                }
            }
// ReSharper disable EmptyGeneralCatchClause
            catch (Exception)
// ReSharper restore EmptyGeneralCatchClause
            {
            }

            IEnumerable<StudentGradebookEntry> entries = null;
            request = CreateWebRequest(string.Format(studentGradebookEntriesEndpoint, score.GradebookEntryId), token);
            try
            {
                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    if (response != null)
                    {
                        using (var responseStream = response.GetResponseStream())
                        {
                            entries = JsonHelper.Deserialize<StudentGradebookEntry[]>(responseStream);
                        }
                    }
                }
            }
// ReSharper disable EmptyGeneralCatchClause
            catch (Exception)
// ReSharper restore EmptyGeneralCatchClause
            {
            }

            var studentGradebookEntryId = 
                (from entry in entries where entry.StudentId == score.StudentId select entry.Id).FirstOrDefault();

            if (studentGradebookEntryId == null)
            {
                CreateStudentGradebookEntry(sectionId, score, token);
            }
            else
            {
                UpdateStudentGradebookEntry(sectionId, studentGradebookEntryId, score, token);
            }
        }

        public static void UpdateStudentGradebookEntry(string sectionId, string studentGradebookEntryId, AssignmentScore score, string token)
        {
            const string studentGradebookEntriesEndpoint =
                "https://api.sandbox.inbloom.org/api/rest/v1.1/studentGradebookEntries/{0}";

            var associations = GetStudentSectionAssociations(sectionId, token);
            var association = associations.FirstOrDefault(a => a.StudentId == score.StudentId);

            var studentGradebookEntry = new StudentGradebookEntry
            {
                DateFulfilled = DateTime.Now,
                GradebookEntryId = score.GradebookEntryId,
                NumericGradeEarned = score.NumericGradeEarned,
                SectionId = sectionId,
                StudentId = score.StudentId,
                StudentSectionAssociationId = association == null ? null : association.Id
            };

            var request = CreateWebRequest(string.Format(studentGradebookEntriesEndpoint, studentGradebookEntryId), token);
            request.Method = "PUT";
            try
            {
                using (var requestStream = request.GetRequestStream())
                {
                    JsonHelper.Serialize(requestStream, studentGradebookEntry);
                }
                using (request.GetResponse() as HttpWebResponse)
                {
                }
            }
// ReSharper disable EmptyGeneralCatchClause
            catch
// ReSharper restore EmptyGeneralCatchClause
            {
            }
        }
    }
}

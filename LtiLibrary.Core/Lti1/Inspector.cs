using System;
using System.Collections.Generic;
using LtiLibrary.Core.Common;

namespace LtiLibrary.Core.Lti1
{
    public static class Inspector
    {
        public static InspectionResult[] Inspect(LtiRequest ltiRequest, Inspection[] inspections)
        {
            var results = new List<InspectionResult>();
            foreach (var inspection in inspections)
            {
                switch (inspection)
                {
                    case Inspection.Outcomes10:
                        if (!string.IsNullOrWhiteSpace(ltiRequest.LisOutcomeServiceUrl))
                        {
                            Uri url;
                            if (!Uri.TryCreate(ltiRequest.LisOutcomeServiceUrl, UriKind.RelativeOrAbsolute, out url))
                            {
                                results.Add(new InspectionResult
                                {
                                    Message = $"Invalid {LtiConstants.LisOutcomeServiceUrlParameter}.",
                                    Severity = InspectionSeverity.Error
                                });
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(ltiRequest.LisOutcomeServiceUrl))
                        {
                            if (string.IsNullOrWhiteSpace(ltiRequest.LisResultSourcedId))
                            {
                                results.Add(new InspectionResult
                                {
                                    Message = $"Missing {LtiConstants.LisResultSourcedIdParameter}.",
                                    Severity = InspectionSeverity.Warning
                                });
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(ltiRequest.LisResultSourcedId))
                        {
                            if (string.IsNullOrWhiteSpace(ltiRequest.LisOutcomeServiceUrl))
                            {
                                results.Add(new InspectionResult
                                {
                                    Message = $"Missing {LtiConstants.LisOutcomeServiceUrlParameter}.",
                                    Severity = InspectionSeverity.Warning
                                });
                            }
                        }
                        break;
                }
            }
            return results.ToArray();
        }
    }
    public enum Inspection
    {
        Outcomes10
    }

    public class InspectionResult
    {
        public InspectionSeverity Severity { get; set; }
        public string Message { get; set; }
    }

    public enum InspectionSeverity
    {
        Error,
        Warning
    }
}

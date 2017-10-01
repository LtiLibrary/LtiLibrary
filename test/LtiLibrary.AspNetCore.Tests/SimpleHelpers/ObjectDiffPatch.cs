#region *   License     *
/*
    SimpleHelpers - ObjectDiffPatch   

    Copyright © 2014 Khalid Salomão

    Permission is hereby granted, free of charge, to any person
    obtaining a copy of this software and associated documentation
    files (the “Software”), to deal in the Software without
    restriction, including without limitation the rights to use,
    copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the
    Software is furnished to do so, subject to the following
    conditions:

    The above copyright notice and this permission notice shall be
    included in all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND,
    EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
    OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
    NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
    HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
    WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
    FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
    OTHER DEALINGS IN THE SOFTWARE. 

    License: http://www.opensource.org/licenses/mit-license.php
    Website: https://github.com/khalidsalomao/SimpleHelpers.Net
 */
#endregion

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LtiLibrary.AspNetCore.Tests.SimpleHelpers
{    
    public class ObjectDiffPatch
    {
        private const string PrefixArraySize = "@@ Count";
        private const string PrefixRemovedFields = "@@ Removed";

        /// <summary>
        /// Compares two objects and generates the differences between them.
        /// </summary>
        /// <typeparam name="T">The type of the T.</typeparam>
        /// <param name="original">The original.</param>
        /// <param name="updated">The updated.</param>
        /// <returns></returns>
        public static ObjectDiffPatchResult GenerateDiff<T> (T original, T updated) where T : class
        {
            // ensure the serializer will not ignore null values
            var writer = GetJsonSerializer ();
            // parse our objects
            JObject originalJson, updatedJson;
            if (typeof (JObject).IsAssignableFrom (typeof (T)))
            {
                originalJson = original as JObject;
                updatedJson = updated as JObject;                
            }
            else
            {
                originalJson = original != null ? JObject.FromObject (original, writer) : null;
                updatedJson = updated != null ? JObject.FromObject (updated, writer) : null;
            }
            // analyse their differences!
            var result =  Diff (originalJson, updatedJson);
            result.Type = typeof (T);
            return result;
        }

        /// <summary>
        /// Modifies an object according to a diff.
        /// </summary>
        /// <typeparam name="T">The type of the T.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="diffJson">The diff json.</param>
        /// <returns></returns>
        public static T PatchObject<T> (T source, string diffJson) where T : class
        {
            var diff = JObject.Parse (diffJson);
            return PatchObject (source, diff);
        }

        /// <summary>
        /// Modifies an object according to a diff.
        /// </summary>
        /// <typeparam name="T">The type of the T.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="diffJson">The diff json.</param>
        /// <returns></returns>
        public static T PatchObject<T> (T source, JObject diffJson) where T : class
        {
            var sourceJson = source != null ? JObject.FromObject (source, GetJsonSerializer ()) : null;
            var resultJson = Patch (sourceJson, diffJson);

            return resultJson?.ToObject<T> ();
        }

        private static ObjectDiffPatchResult Diff (JObject source, JObject target)
        {
            ObjectDiffPatchResult result = new ObjectDiffPatchResult ();
            // check for null values
            if (source == null && target == null)
            {
                return result;
            }
            else if (source == null || target == null)
            {
                result.OldValues = source;
                result.NewValues = target;
                return result;
            }

            // compare internal fields           
            JArray removedNew = new JArray ();
            JArray removedOld = new JArray ();
            JToken token;
            // start by iterating in source fields
            foreach (var i in source)
            {
                // check if field exists
                if (!target.TryGetValue (i.Key, out token))
                {
                    AddOldValuesToken (result, i.Value, i.Key);
                    removedNew.Add (i.Key);
                }
                // compare field values
                else
                {
                    DiffField (i.Key, i.Value, token, result);
                }
            }
            // then iterate in target fields that are not present in source
            foreach (var i in target)
            {
                // ignore alredy compared values
                if (source.TryGetValue (i.Key, out token))
                    continue;
                // add missing tokens
                removedOld.Add (i.Key);
                AddNewValuesToken (result, i.Value, i.Key);
            }

            if (removedOld.Count > 0)
                AddOldValuesToken (result, removedOld, PrefixRemovedFields);
            if (removedNew.Count > 0)
                AddNewValuesToken (result, removedNew, PrefixRemovedFields);

            return result;
        }

        private static void DiffField (string fieldName, JToken source, JToken target, ObjectDiffPatchResult result = null)
        {
            if (result == null)
                result = new ObjectDiffPatchResult ();
            if (source == null)
            {
                if (target != null)
                {
                    AddToken (result, fieldName, null, target);
                }
            }
            else if (target == null)
            {
                AddToken (result, fieldName, source, null);
            }
            else if (source.Type == JTokenType.Object)
            {
                var v = target as JObject;
                var r = Diff (source as JObject, v);
                if (!r.AreEqual)
                    AddToken (result, fieldName, r);
            }
            else if (source.Type == JTokenType.Array)
            {
                var aS = (source as JArray);
                var aT = (target as JArray);

                if (aS == null || aT == null)
                {
                    AddToken(result, fieldName, source, target);
                }
                else if ((aS.Count == 0 || aT.Count == 0) && (aS.Count != aT.Count))
                {
                    AddToken (result, fieldName, source, target);
                }
                else
                {
                    ObjectDiffPatchResult arrayDiff = new ObjectDiffPatchResult ();
                    int minCount = Math.Min (aS.Count, aT.Count);
                    for (int i = 0; i < Math.Max (aS.Count, aT.Count); i++)
                    {
                        if (i < minCount)
                        {
                            DiffField (i.ToString (), aS[i], aT[i], arrayDiff);
                        }
                        else if (i >= aS.Count)
                        {
                            AddNewValuesToken (arrayDiff, aT[i], i.ToString ());
                        }
                        else
                        {
                            AddOldValuesToken (arrayDiff, aS[i], i.ToString ());
                        }
                    }

                    if (!arrayDiff.AreEqual)
                    {
                        if (aS.Count != aT.Count)
                            AddToken (arrayDiff, PrefixArraySize, aS.Count, aT.Count);
                        AddToken (result, fieldName, arrayDiff);
                    }
                }
            }
            else
            {
                if (!JToken.DeepEquals (source, target))
                {
                    AddToken (result, fieldName, source, target);
                }
            }
        }
                
        private static JsonSerializer GetJsonSerializer ()
        {
            // ensure the serializer will not ignore null values
            var settings = JsonConvert.DefaultSettings != null ? JsonConvert.DefaultSettings () : new JsonSerializerSettings ();
            settings.NullValueHandling = NullValueHandling.Include;
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            settings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
            settings.Formatting = Formatting.None;
            settings.MissingMemberHandling = MissingMemberHandling.Ignore;
            settings.ObjectCreationHandling = ObjectCreationHandling.Replace;
			
            // create our custom serializer
            var writer = JsonSerializer.Create (settings);
            return writer;
        }

        private static JToken Patch (JToken sourceJson, JToken diffJson)
        {
            // deal with null values
            if (sourceJson == null || diffJson == null)
            {
                return diffJson;
            }
            else if (diffJson.Type != JTokenType.Object)
            {
                return diffJson;
            }
            // deal with objects
            else
            {
                JObject diffObj = (JObject)diffJson;
                JToken token;
                if (sourceJson.Type == JTokenType.Array)
                {                    
                    int sz = 0;
                    bool foundArraySize = diffObj.TryGetValue(PrefixArraySize, out token);
                    if (foundArraySize)
                    {
                        diffObj.Remove (PrefixArraySize);
                        sz = token.Value<int> ();                        
                    }
                    var array = sourceJson as JArray;
                    // resize array
                    if (array != null && (foundArraySize && array.Count != sz))
                    {
                        JArray snapshot = array.DeepClone () as JArray;
                        array.Clear ();
                        for (int i = 0; i < sz; i++)
                        {
                            array.Add (snapshot != null && i < snapshot.Count ? snapshot[i] : null);
                        }
                    }
                    // patch it
                    foreach (var f in diffObj)
                    {
                        if (Int32.TryParse (f.Key, out var ix))
                        {
                            if (array != null) array[ix] = Patch (array[ix], f.Value);
                        }
                    }
                }
                else
                {
                    var sourceObj = sourceJson as JObject ?? new JObject();
                    // remove fields
                    if (diffObj.TryGetValue (PrefixRemovedFields, out token))
                    {
                        diffObj.Remove (PrefixRemovedFields);
                        var jArray = token as JArray;
                        if (jArray != null)
                            foreach (var f in jArray)
                                sourceObj.Remove (f.ToString ());
                    }

                    // patch it
                    foreach (var f in diffObj)
                    {
                        sourceObj[f.Key] = Patch (sourceObj[f.Key], f.Value);
                    }
                }
            }
            return sourceJson;
        }

        private static void AddNewValuesToken (ObjectDiffPatchResult item, JToken newToken, string fieldName)
        {
            if (item.NewValues == null)
                item.NewValues = new JObject ();
            item.NewValues[fieldName] = newToken;
        }

        private static void AddOldValuesToken (ObjectDiffPatchResult item, JToken oldToken, string fieldName)
        {
            if (item.OldValues == null)
                item.OldValues = new JObject ();
            item.OldValues[fieldName] = oldToken;
        }

        private static void AddToken (ObjectDiffPatchResult item, string fieldName, JToken oldToken, JToken newToken)
        {
            AddOldValuesToken (item, oldToken, fieldName);

            AddNewValuesToken (item, newToken, fieldName);
        }

        private static void AddToken (ObjectDiffPatchResult item, string fieldName, ObjectDiffPatchResult diff)
        {
            AddToken (item, fieldName, diff.OldValues, diff.NewValues);
        }
    }

    /// <summary>
    /// Result of a diff operation between two objects
    /// </summary>
    public class ObjectDiffPatchResult
    {
        private JObject _oldValues;

        private JObject _newValues;

        /// <summary>
        /// If the compared objects are equal.
        /// </summary>
        /// <value>true if the obects are equal; otherwise, false.</value>
        public bool AreEqual => _oldValues == null && _newValues == null;

        /// <summary>
        /// The type of the compared objects.
        /// </summary>
        /// <value>The type of the compared objects.</value>
        public Type Type { get; set; }

        /// <summary>
        /// The values modified in the original object.
        /// </summary>
        public JObject OldValues
        {
            get => _oldValues;
            set => _oldValues = value;
        }

        /// <summary>
        /// The values modified in the updated object.
        /// </summary>
        public JObject NewValues
        {
            get => _newValues;
            set => _newValues = value;
        }
    }

    class ObjectDiffPatchJTokenComparer : IEqualityComparer<JToken>
    {
        public bool Equals (JToken x, JToken y)
        {
            if (x == null && y == null)
                return true;
            if (x == null || y == null)
                return false;
            return JToken.DeepEquals (x, y);
        }
        public int GetHashCode (JToken i)
        {
            return i.ToString ().GetHashCode ();
        }
    }
}

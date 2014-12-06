namespace LtiLibrary.Lti1
{
    public interface IBasicLaunchRequest : ILtiRequest
    {
        /// <summary>
        /// This field should be no more than 1023 characters long. 
        /// This value should not change from one launch to the next and in general, the TP can expect that 
        /// there is a one-to-one mapping between the lis_outcome_service_url and a particular oauth_consumer_key. 
        /// This value might change if there was a significant re-configuration of the TC system or if the TC 
        /// moved from one domain to another. 
        /// The TP can assume that this URL generally does not change from one launch to the next but should be 
        /// able to deal with cases where this value rarely changes. 
        /// The service URL may support various operations/actions. 
        /// The Basic Outcomes Service Provider will respond with a response of 'unimplemented' for actions it 
        /// does not support. 
        /// <para>
        /// Parameter: lis_outcome_service_url.
        /// Custom parameter substitution: none.
        /// Versions: 1.1, 1.2.
        /// Required if the TC is accepting outcomes for any launches associated with the resource_link_id.
        /// </para>
        /// </summary>
        string LisOutcomeServiceUrl { get; set; }

        /// <summary>
        /// This field contains an identifier that indicates the LIS Result Identifier (if any) associated with this launch. 
        /// This field identifies a unique row and column within the TC gradebook. 
        /// This field is unique for every combination of context_id/resource_link_id/user_id. 
        /// This value may change for a particular resource_link_id/user_id from one launch to the next. 
        /// The TP should only retain the most recent value for this field for a particular resource_link_id/user_id.
        /// <para>
        /// Parameter: lis_result_sourcedid.
        /// Custom parameter substitution: $Result.sourcedGUID.
        /// Versions: 1.1, 1.2.
        /// Optional.
        /// </para>
        /// </summary>
        string LisResultSourcedId { get; set; }

        /// <summary>
        /// A plain text description of the link’s destination, suitable for display alongside the link. 
        /// Typically no more than a few lines long.
        /// <para>
        /// Parameter: resource_link_description.
        /// Custom parameter substitution: n/a.
        /// Versions: 1.0, 1.1, 1.2.
        /// Optional.
        /// </para>
        /// </summary>
        string ResourceLinkDescription { get; set; }

        /// <summary>
        /// This is an opaque unique identifier that the TC guarantees will be unique within the TC for every placement of the link. 
        /// If the tool/activity is placed multiple times in the same context, each of those placements will be distinct. 
        /// This value will also change if the item is exported from one system or context and imported into another system or context.
        /// <para>
        /// Parameter: resource_link_id.
        /// Custom parameter substitution: n/a.
        /// Versions: 1.0, 1.1, 1.2.
        /// Required.
        /// </para>
        /// </summary>
        string ResourceLinkId { get; set; }

        /// <summary>
        /// A plain text title for the resource. This is the clickable text that appears in the link.
        /// <para>
        /// Parameter: resource_link_title.
        /// Custom parameter substitution: n/a.
        /// Versions: 1.0, 1.1, 1.2.
        /// Recommended.
        /// </para>
        /// </summary>
        string ResourceLinkTitle { get; set; }

        /// <summary>
        /// A comma separated list of the user_id values which the current user can access as a mentor. 
        /// The typical use case for this parameter is where the Mentor role represents a parent, guardian or auditor. 
        /// It may be used in different ways by each TP, but the general expectation is that the mentor will be provided with
        /// access to tracking and summary information, but not necessarily the user’s personal content or assignment submissions. 
        /// In order to accommodate user_id values which contain a comma, each user_id should be url-encoded. 
        /// This also means that each user_id from the comma separated list should url-decoded before a TP uses it. 
        /// This parameter should only be used when one of the roles passed for the current user is for urn:lti:role:ims/lis/Mentor.
        /// <para>
        /// Parameter: role_scope_mentor.
        /// Custom parameter substitution: n/a.
        /// Versions: 1.1, 1.2.
        /// Optional.
        /// </para>
        /// </summary>
        string RoleScopeMentor { get; set; }
    }
}
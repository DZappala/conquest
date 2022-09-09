using UnityEngine;

public class Organization : ScriptableObject
{
    //TODO implement organizations
    public string title;

    public string followers;

    public enum OrganizationType
    {
        PoliticalParty,
        Religion,
        Guild
    }
}

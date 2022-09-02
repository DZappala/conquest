using System;
using System.Collections;
using System.Collections.Generic;
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

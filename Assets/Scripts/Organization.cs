using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Organization : ScriptableObject 
{
    public string title;

    public string followers;

    public enum OrganizationType
    {
        PoliticalParty,
        Religion,
        Guild
    }
}
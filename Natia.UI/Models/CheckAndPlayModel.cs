using Natia.Core.Entities;

namespace Natia.UI.Models;

public class CheckAndPlayModel
{
    public required string WhatNatiaSaid { get; set; }


    public bool IsError { get; set; } = false;


    public bool IsCritical { get; set; } = false;


    public Topic WhatWasTopic { get; set; }


    public Priority Priority { get; set; }


    public string? ChannelName { get; set; }


    public string? Satellite { get; set; }


    public string? SuggestedSolution { get; set; }


    public string? ErrorMessage { get; set; }


    public string? ErrorDetails { get; set; }
}

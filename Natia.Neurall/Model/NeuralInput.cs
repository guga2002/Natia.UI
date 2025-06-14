using Natia.Core.Entities;

namespace Natia.Neurall.Model;

public class NeuralInput
{
    public string? ChannelName { get; set; }

    public string? Satellite { get; set; }

    public string? ErrorMessage { get; set; }

    public string? ErrorDetails { get; set; }

    public Topic? WhatWasTopic { get; set; }

    public Priority? Priority { get; set; }
}

﻿using RcCloud.DateScraper.Domain.Races;

namespace RcCloud.FunctionApi.Races.Dto;

public class RaceCategoryDto(string key, List<RaceMeeting> races)
{
    public string Key { get; } = key;

    public List<RaceMeeting> Races { get; } = races;
}

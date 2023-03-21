namespace ThreeInOne.Models.SunInfo;
public record LocalSunInfo(
    TimeSpan Sunrise,
    TimeSpan Sunset,
    string Location,
    TimeSpan DayLenght,
    DateTime MeasuredAt);
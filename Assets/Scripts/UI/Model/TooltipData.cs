using UnityEngine;

public class TooltipData
{
    public string IconAddress {  get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string UsageHint { get; private set; }
    public string CountText { get; private set; }

    public TooltipData(string iconAddress, string name, string description, string usageHint, string countText)
    {
        IconAddress = iconAddress;
        Name = name;
        Description = description;
        UsageHint = usageHint;
        CountText = countText;
    }
}

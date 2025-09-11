namespace YAMDCC.Service.RegLayouts;

internal sealed class RegLayoutV2 : IRegLayout
{
    public byte ChargeLimReg { get; } = 0xD7;
    public byte PerfModeReg { get; } = 0xD2;
    public byte FanModeReg { get; } = 0xD4;
    public byte KeyLightReg { get; } = 0xD3;
    public byte? KeySwapReg { get; } = 0xE8;
}

namespace YAMDCC.Service.RegLayouts;

internal sealed class RegLayoutV1 : IRegLayout
{
    public byte ChargeLimReg { get; } = 0xEF;
    public byte PerfModeReg { get; } = 0xF2;
    public byte FanModeReg { get; } = 0xF4;
    public byte KeyLightReg { get; } = 0xF3;

    // Win/Fn swap is not supported on WMI1 ECs
    public byte? KeySwapReg { get; }

}

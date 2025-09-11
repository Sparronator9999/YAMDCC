namespace YAMDCC.Service.RegLayouts;

internal interface IRegLayout
{
    byte ChargeLimReg { get; }
    byte PerfModeReg { get; }
    byte FanModeReg { get; }
    byte KeyLightReg { get; }
    byte? KeySwapReg { get; }
}

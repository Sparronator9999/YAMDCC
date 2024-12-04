// This file is part of YAMDCC (Yet Another MSI Dragon Center Clone).
// Copyright © Sparronator9999 2023-2024.
//
// YAMDCC is free software: you can redistribute it and/or modify it
// under the terms of the GNU General Public License as published by the Free
// Software Foundation, either version 3 of the License, or (at your option)
// any later version.
//
// YAMDCC is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for
// more details.
//
// You should have received a copy of the GNU General Public License along with
// YAMDCC. If not, see <https://www.gnu.org/licenses/>.

namespace YAMDCC.GUI
{
    internal class Status
    {
        internal StatusCode Code;
        internal int RepeatCount;

        internal Status()
        {
            Code = StatusCode.None;
            RepeatCount = 0;
        }

        internal Status(StatusCode code, int repeatCount)
        {
            Code = code;
            RepeatCount = repeatCount;
        }
    }

    internal enum StatusCode
    {
        None = 0,
        ServiceCommandFail,
        ServiceResponseEmpty,
        ServiceTimeout,
        ConfLoading,
        NoConfig,
        ConfApplySuccess,
        FullBlastToggleSuccess,
    }
}

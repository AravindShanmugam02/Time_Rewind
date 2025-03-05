using System.Collections.Generic;

public interface ITimer
{
    void ITimerRewindingUpdate(TimeKeeper.RewindState _rewindState);
    void ITimerContObjInfoUpdate(ControllableObjectInfo _cOI);

    //void ITimerContObjInfoListUpdate(List<ControllableObjectInfo> _cOIList); // Not required now
}

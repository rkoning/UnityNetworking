using System.Collections;
using UnityEngine;

public abstract class StatusFactory: ScriptableObject {
    public abstract Status GetStatus(Health target, Avatar source);
}

public class StatusFactory<DataType, StatusType>: StatusFactory
    where StatusType: Status<DataType>, new() {
    public DataType data;

    public override Status GetStatus(Health target, Avatar source)
    {
        return new StatusType { data = this.data, target = target };
    }
}

public abstract class Status {
    public abstract void Apply();

    public abstract void PerTick();

    public abstract void Unapply();

    public IEnumerator ApplyOvertime(float duration) {
      float delta = duration / 25f; // for now default tick time is 0.25 sec     
      float now = Time.fixedTime;
      float end = now + duration;

      float last = 0f;
      while (now < end) {
         now = Time.fixedTime;
         if (now > last + delta) {                       
            PerTick();
            last = now;
         }
         yield return null;                           
      }

      Unapply();
    }
}

public abstract class Status<DataType>: Status {
    public DataType data;
    public Health target;
    public Avatar source;
}
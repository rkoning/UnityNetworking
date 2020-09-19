using System.Collections;
using UnityEngine;

public abstract class StatusFactory: ScriptableObject {
    public abstract Status GetStatus(Health target, BaseAvatar source);
}

public class StatusFactory<T, StatusType>: StatusFactory 
    where T : StatusData
    where StatusType: Status<T>, new() {
    public T data;

    public override Status GetStatus(Health target, BaseAvatar source)
    {
        return new StatusType { data = this.data, target = target };
    }
}

public abstract class StatusData {
    public string name;
    public Sprite sprite;
}

public abstract class Status {
    public abstract StatusData GetData();
    public delegate void UnapplyEvent();
    public event UnapplyEvent OnUnapply;
    public virtual void Apply() {
        OnUnapply += () => {};
    }

    public abstract void PerTick();

    public virtual void Unapply() {
        OnUnapply();
    }

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

public abstract class Status<T>: Status where T : StatusData {
    public T data;
    public Health target;
    public Avatar source;

    public override StatusData GetData() {
        return data;
    }
}
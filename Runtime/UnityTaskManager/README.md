
// This is a new coroutine interface for Unity.
//
// The motivation for this is twofold:
//
// 1. The existing coroutine API provides no means of stopping specific
//    coroutines; StopCoroutine only takes a string argument, and it stops
//    all coroutines started with that same string; there is no way to stop
//    coroutines which were started directly from an enumerator.  This is
//    not robust enough and is also probably pretty inefficient.
//
// 2. StartCoroutine and friends are MonoBehaviour methods.  This means
//    that in order to start a coroutine, a user typically must have some
//    component reference handy.  There are legitimate cases where such a
//    constraint is inconvenient.  This implementation hides that
//    constraint from the user.
//
// And another two benefit by using my derivation code including:
//
// 1. It's Garbage-Collection free; the original workflow uses New() keyword
//    for every new Coroutiones, which means that after that coroutine finished,
//    This class isn't get recycled, resulting a new GC allocation. This also means
//    that by using this derived-code, resulting an increase in performance
//    on both editor and real devices.
//
// 2. the Implementation by IEnumerable isn't efficient enough if it used for many times.
//    Most of the time, we use Corountines for tweening a parameter. And to create one,
//    Takes a time to think the math behind the scene. This is C#, and there's why delegates
//    exist. With this code, You can implement Delegate (especially it's ground breaking feature,
//    Anonymous method, google for that) to wrap up several lines to just a single line, while
//    this class handle the math for the time function. See below for futher example.
//
// Example usage:
//
// ----------------------------------------------------------------------------
// IEnumerator MyAwesomeTask()
// {
//     while(true) {
//         Debug.Log("Logcat iz in ur consolez, spammin u wif messagez.");
//         yield return null;
///    }
// }
//
// IEnumerator TaskKiller(float delay, Task t)
// {
//     yield return new WaitForSeconds(delay);
//     t.Stop();
// }
//
// void SomeCodeThatCouldBeAnywhereInTheUniverse()
// {
//     Task spam = new Task(MyAwesomeTask());
//     new Task(TaskKiller(5, spam));
// }
// ----------------------------------------------------------------------------
//
// When SomeCodeThatCouldBeAnywhereInTheUniverse is called, the debug console
// will be spammed with annoying messages for 5 seconds.
//
// Simple, really.  There is no need to initialize or even refer to TaskManager.
// When the first Task is created in an application, a "TaskManager" GameObject
// will automatically be added to the scene root with the TaskManager component
// attached.  This component will be responsible for dispatching all coroutines
// behind the scenes.
//
// Task also provides an event that is triggered when the coroutine exits.
//
// ----------------------------------------------------------------------------
//
// And as a said before, constant use of New() keyword will result on a significant
// GC Allocations. this problem can be addressed by reusing the class after it's
// current Coroutine finished. This is somewhat complex in theory, but, don't worry,
// Here we use the implemention of a Class Pooler from what I'm use for my TEXDraw package.
// To make the optimization takes effect, just replace from "new Task" to "Task.Get":
//
// void SomeCodeThatCouldBeAnywhereInTheUniverse()
// {
//     Task spam = Task.Get(MyAwesomeTask());
//     Task.Get(TaskKiller(5, spam));
// }

# original

# TaskManager
A Futher implementation of Coroutines in Unity3D

This implementation is a derived implementation from 
https://github.com/krockot/Unity-TaskManager

Instead of these codes:

```C#
void Start() {
  StartCoroutine(Loop20Sec());
}

void OnDisable() {
  StopCoroutine(Loop20Sec());
}

IEnumerator Loop20Sec() {
  float tim = Time.time + 20;
		while (tim > Time.time) {
		  float progress = 1 - ((tim - Time.time) / totalTim);
		  //Moving from (0,0,0) to (1,0,0)
		  transform.position = new Vector3(progress,0,0);
  }
}
```

why don't just wrap up that coroutine to a single code?

```C#
Task c;
void Start() {
  //This make life just get more simpler to tweening things
  c = Task.Get(delegate(float t) { transform.position = new Vector3(t,0,0); }, 20);
}

void OnDisable() {
  c.Stop();
}
```
not just using delegates, this Task also support pause/resume system, And better handling for specific Task,
Which is a feature from the base code (https://github.com/krockot/Unity-TaskManager). Additionally, when it's 
got unused, it's resources (memory) will automatically kept for later usage, this implementation based from my
[TEXDraw](http://u3d.as/mFe) package. This also means that task will performs ss fast as possible, without 
suffer from GC Allocations.

See sources for more example and documentation.

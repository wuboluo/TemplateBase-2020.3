using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Bridge is available only in luna compilation time, so wrap it in define.
#if UNITY_LUNA
using Bridge;

// It will be replaced in JavaScript with library code.
// Name is important to pass existing class in JS.
[External][Name( "pc.MyTextListener" )]
#endif
public class MyTextListener
{
    public extern string EnterKey(string ctx);
    public extern string LoadingComplete();
    public extern string Engagement();

    public extern string Tutorial();

    public extern string Win();

    public extern string Lose();

    public extern string Resize();

    public extern string Finish();

    public extern string GoStore();

    public extern string Customfunction(string message);

    public extern int Addnumber();
    public extern int Addnumber1();

    public extern int Addnumber2();

    public extern bool Isluna();
    public extern string CopyAA(string message);
}

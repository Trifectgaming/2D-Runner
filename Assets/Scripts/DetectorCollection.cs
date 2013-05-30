using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public class DetectorCollection : IEnumerable<Detector>
{
    public Detector Prefab;
    public Detector[] Detectors = new Detector[4];
    public Detector Front;
    public Detector Back;
    public Detector Top;
    public Detector Bottom;

    private Transform FrontTrans;
    private Transform BackTrans;
    private Transform TopTrans;
    private Transform BottomTrans;

    public void Initialize(Transform host)
    {
        
        Back = Object.Instantiate(Prefab) as Detector;
        Back.name = "DetectorBack";
        BackTrans = Back.transform;
        BackTrans.parent = host;

        Top = Object.Instantiate(Prefab) as Detector;
        Top.name = "DetectorTop";
        TopTrans = Top.transform;
        TopTrans.parent = host;
        
        Front = Object.Instantiate(Prefab) as Detector;
        Front.name = "DetectorFront";
        FrontTrans = Front.transform;
        FrontTrans.parent = host;
       
        Bottom = Object.Instantiate(Prefab) as Detector;
        Bottom.name = "DetectorBottom";
        BottomTrans = Bottom.transform;
        BottomTrans.parent = host;
        
        Detectors[0] = Back;
        Detectors[1] = Top;
        Detectors[2] = Front;
        Detectors[3] = Bottom;
    }
    
    public void Resize(Bounds bounds, Transform host)
    {
        FrontTrans.localScale = new Vector3(.05f, bounds.size.y/2, 1);
        FrontTrans.localPosition = new Vector3(bounds.max.x + .05f, bounds.center.y);
        
        BackTrans.localScale = new Vector3(.05f, bounds.size.y/2, 1);
        BackTrans.localPosition = new Vector3(bounds.min.x - .05f, bounds.center.y);

        TopTrans.localScale = new Vector3(bounds.size.x, .1f, 1);
        TopTrans.localPosition = new Vector3(bounds.center.x, bounds.max.y + .1f);
        
        BottomTrans.localScale = new Vector3(bounds.size.x, .1f, 1);
        BottomTrans.localPosition = new Vector3(bounds.center.x, bounds.min.y - .1f);
    }

    public IEnumerator<Detector> GetEnumerator()
    {
        return ((IEnumerable<Detector>)Detectors).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return Detectors.GetEnumerator();
    }
}
# kurvz
Scripts for using cubic splines and bezier curves

 ![catmullrom](/spline1.gif)

## What is this?
Some scripts for moving objects smoothly between any points in 3-space. For example, if I want an objec tto move at a steady speed between points *(0,0,0), (10,-10, 10), (20, 0, 300), (15, 100, -100)* , then I can use the `CatmullRomSpline` class to do this. The object does not move in as traight line between the points and make a sharp turn to get to the next point, like a waypoint system. The object follows a nice, smooth curve.

## Example
I want my object to travel between (Vector3) points *p1,p2,p3,p4,p5* on a smooth curve, at speed 10 units/sec.
First we have to wrap Vector3 in a SplineVector interface (not only Vector3, any type will do, it just needs to return a Vector3 when asked.
```

_mySpline = new CatmullRomSpline(mySplineVectorList, onFinishCallback);

.
.
.
//and in the update

this.transform.position = _mySpline.UpdateAtSpeed(Time.deltaTime, 10);
```
That's it.

You can also add new points to the CatmullRomSpline so the object can move forever, because it will have new points to travel to. (Points already visited are removed).

see *SphereBehaviour.cs* for a working example.


## Voronoi Diagram

We can create a *Voronoi Diagram* for a set of points using the `VoronoiGenerator` class. For example, look at the following gif

![vornoi](/voronoi1.gif)
 
This shows the *SceneView* of a 10 points, which move randomly every frame. The voronoi diagram, shown by the lines, (`Debug.DrawLine()`) is calcualted each frame. (The per-frame calculation is a little bit heavy)

### How to use
The `VoronoiGenerator` will only accept `VectorNode` objects, so you can wrap `Vector2` in one of these (Vornoi Diagrams are 2D, so don't try to use `Vector3`).
```
VoronoiGenerator vornoiGen = new VoronoiGenerator(myListOfVectorNodes);
voronoiGen.CreateDiagram();
```
THis will creat ethe list of points and edges that can be used to construct the voronoi diagram. The diagram can be updated by calling
`voronoiGen.Rebuild();`
This will recalculate all data for `myListOfVectorNodes`, so if the *(x,y)* coordinates have changed, then the vornoi diagram will also change.

#### 100-Point Voronoi
Example of voronoi diagram for 100 points (not recalculated every frame because it would be too heavy)

![vornoi2](/voronoi2.png)

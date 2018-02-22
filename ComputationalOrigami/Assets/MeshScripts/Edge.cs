using System;

public class Edge : IComparable<Edge>
{
    int vertice1;
    int vertice2;

    public Edge(int v1, int v2)
    {
        vertice1 = v1;
        vertice2 = v2;
    }

    public int CompareTo(Edge other)
    {
        return vertice1.CompareTo(other.getV1());
    }

    public int getV1()
    {
        return vertice1;
    }

    public int getV2()
    {
        return vertice2;
    }

    public override bool Equals(object o)
    {
        if (o.GetType() != typeof(Edge) ) {
            return false;
        }

        Edge other = (Edge) o;
        if ((other.getV1() == vertice1 && other.getV2() == vertice2)
            || (other.getV1() == vertice2 && other.getV1() == vertice1)) {
            return true;
        } else {
            return false;
        }
    }

    override public string ToString()
    {
        return "v1 = " + vertice1 +
            "\nv2 = " + vertice2;
    }



    


}
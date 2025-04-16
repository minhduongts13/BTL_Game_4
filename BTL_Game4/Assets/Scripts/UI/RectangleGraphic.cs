using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;
using UnityEngine.Rendering;

[RequireComponent(typeof(CanvasRenderer))]
public class RectangleGraphic : Graphic
{
    // Màu sắc sẽ lấy từ `color` của Graphic
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        Rect r = GetPixelAdjustedRect();

        // 4 góc của hình chữ nhật
        Vector2[] corners = new Vector2[4] {
            new Vector2(r.xMin, r.yMin),
            new Vector2(r.xMin, r.yMax),
            new Vector2(r.xMax, r.yMax),
            new Vector2(r.xMax, r.yMin)
        };

        UIVertex vt = UIVertex.simpleVert;
        vt.color = color;
        vt.uv0 = Vector2.zero;

        // Thêm 4 đỉnh
        for (int i = 0; i < 4; i++)
        {
            vt.position = corners[i];
            vh.AddVert(vt);
        }
        // 2 tam giác tạo thành hình chữ nhật
        vh.AddTriangle(0, 1, 2);
        vh.AddTriangle(2, 3, 0);
    }
}

/*
=============================================================================

Basicamente faz com que o cursor siga a posição do mouse na tela, mas em 3D.
Se a propriedade shouldOnlySetPosition for true, ele vai setar a posição do cursor
e vai rotacionar ele para olhar para a superfície onde o mouse está apontando.
Se for false, ele vai setar a posição do cursor para uma posição fixa na frente da
câmera, mas vai rotacionar ele para olhar para a posição da câmera.

Author: Caveirinha
Date: 30/06/2025
=============================================================================
*/


using Godot;
using Godot.Collections;

public partial class Cursor : Sprite3D
{
    [Export(PropertyHint.Range, "0, 1000")] public float depth = 10.0f;  // Distância do cursor em relação à câmera
    private Camera3D camera => GetParent() as Camera3D;
    [Export] public bool shouldOnlySetPosition;

    public override void _Process(double delta)
    {
        var mousePosition = GetViewport().GetMousePosition();
        var from = camera.ProjectRayOrigin(mousePosition);
        var direction = camera.ProjectRayNormal(mousePosition);
        var to = from + direction * depth;

        if (shouldOnlySetPosition)
        {
            var spaceState = GetWorld3D().DirectSpaceState;
            var result = spaceState.IntersectRay(new PhysicsRayQueryParameters3D
            {
                From = from,
                To = to,
                CollisionMask = 1,
                Exclude = new Array<Rid> { camera.GetCameraRid() }
            });

            if (result.Count > 0)
            {
                GlobalPosition = (Vector3)result["position"];
                var normal = (Vector3)result["normal"];
                Vector3 up = Vector3.Up;
                // Puta que pariu cara eu odeio fisica
                if (normal.Normalized().IsEqualApprox(Vector3.Up) || normal.Normalized().IsEqualApprox(-Vector3.Up))
                {
                    up = Vector3.Forward;
                }
                LookAt(GlobalPosition + normal, up);
            }
            else
            {
                GlobalPosition = to;
                LookAtFromPosition(GlobalPosition, camera.GlobalPosition, Vector3.Up);
            }
        }
        else
        {
            var camForwardGlobal = -camera.GlobalTransform.Basis.Z;
            var lookPosGlobal = camera.GlobalPosition + camForwardGlobal * depth;
            LookAtFromPosition(GlobalPosition, lookPosGlobal, Vector3.Up);
        }
    }
}

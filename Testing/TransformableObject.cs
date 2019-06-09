﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GL_EditorFramework.EditorDrawables;
using GL_EditorFramework.GL_Core;
using GL_EditorFramework.Interfaces;
using GL_EditorFramework;
using OpenTK;
using static GL_EditorFramework.EditorDrawables.EditorSceneBase;

namespace Testing
{
    class TransformableObject : SingleObject
    {
        public TransformableObject(Vector3 pos)
            : base(pos)
        {

        }

        public Quaternion rotation = Quaternion.Identity;

        public Vector3 scale = new Vector3(1, 1, 1);

        public override Quaternion Rotation { get => rotation; set => rotation = value; }

        public override Vector3 Scale { get => scale; set => scale = value; }

        public override void Draw(GL_ControlModern control, Pass pass, EditorSceneBase editorScene)
        {
            if (pass == Pass.TRANSPARENT)
                return;

            bool hovered = editorScene.Hovered == this;

            control.UpdateModelMatrix(
                Matrix4.CreateScale((Selected ? editorScene.CurrentAction.NewScale(scale) : scale) * 0.5f) *
                Matrix4.CreateFromQuaternion(Selected ? editorScene.CurrentAction.NewRot(rotation) : rotation) *
                Matrix4.CreateTranslation(Selected ? editorScene.CurrentAction.NewPos(position) : position));

            Vector4 blockColor;
            Vector4 lineColor;

            if (hovered && Selected)
                lineColor = hoverColor;
            else if (hovered || Selected)
                lineColor = selectColor;
            else
                lineColor = Color;

            if (hovered && Selected)
                blockColor = Color * 0.5f + hoverColor * 0.5f;
            else if (hovered || Selected)
                blockColor = Color * 0.5f + selectColor * 0.5f;
            else
                blockColor = Color;

            Renderers.ColorBlockRenderer.Draw(control, pass, blockColor, lineColor, control.NextPickingColor());

        }

        public override void Draw(GL_ControlLegacy control, Pass pass, EditorSceneBase editorScene)
        {
            if (pass == Pass.TRANSPARENT)
                return;

            bool hovered = editorScene.Hovered == this;

            control.UpdateModelMatrix(
                Matrix4.CreateScale((Selected ? editorScene.CurrentAction.NewScale(scale) : scale) * 0.5f) *
                Matrix4.CreateFromQuaternion(Selected ? editorScene.CurrentAction.NewRot(rotation) : rotation) *
                Matrix4.CreateTranslation(Selected ? editorScene.CurrentAction.NewPos(position) : position));

            Vector4 blockColor;
            Vector4 lineColor;

            if (hovered && Selected)
                lineColor = hoverColor;
            else if (hovered || Selected)
                lineColor = selectColor;
            else
                lineColor = Color;

            if (hovered && Selected)
                blockColor = Color * 0.5f + hoverColor * 0.5f;
            else if (hovered || Selected)
                blockColor = Color * 0.5f + selectColor * 0.5f;
            else
                blockColor = Color;

            Renderers.ColorBlockRenderer.Draw(control, pass, blockColor, lineColor, control.NextPickingColor());
        }

        public override LocalOrientation GetLocalOrientation(int partIndex)
        {
            return new LocalOrientation(position, rotation);
        }

        public override bool TryStartDragging(DragActionType actionType, int hoveredPart, out LocalOrientation localOrientation, out bool dragExclusively)
        {
            localOrientation = new LocalOrientation(position,rotation);
            dragExclusively = false;
            return Selected;
        }

        public override void SetTransform(Vector3? pos, Quaternion? rot, Vector3? scale, int part, out Vector3? prevPos, out Quaternion? prevRot, out Vector3? prevScale)
        {
            prevPos = null;
            prevRot = null;
            prevScale = null;

            if (pos.HasValue)
            {
                prevPos = position;
                position = pos.Value;
            }

            if (rot.HasValue)
            {
                prevRot = rotation;
                rotation = rot.Value;
            }

            if (scale.HasValue)
            {
                prevScale = this.scale;
                this.scale = scale.Value;
            }
        }

        public override void ApplyTransformActionToSelection(AbstractTransformAction transformAction, ref TransformChangeInfos infos)
        {
            position = transformAction.NewPos(position, out Vector3? prevPos);
            rotation = transformAction.NewRot(rotation, out Quaternion? prevRot);
            scale = transformAction.NewScale(scale, out Vector3? prevScale);
            infos.Add(this, 0, prevPos, prevRot, prevScale);
        }
    }
}

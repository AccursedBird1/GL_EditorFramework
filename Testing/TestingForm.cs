﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GL_EditorFramework;
using GL_EditorFramework.EditorDrawables;
using OpenTK;

namespace Testing
{
    public partial class TestingForm : Form
    {
        public TestingForm()
        {
            InitializeComponent();
        }

        private TestContainer propertyContainer = new TestContainer();

        private EditorScene scene;

        private EditorSceneBase.PropertyChanges propertyChangesAction = new EditorSceneBase.PropertyChanges();

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            scene = new EditorScene();

            listBox1.Items.Add("moving platform");
            scene.objects.Add(new AnimatedObject(new Vector3(0, -4, 0)));

            listBox1.Items.Add("path");
            List<Path.PathPoint> pathPoints = new List<Path.PathPoint>
            {
                new Path.PathPoint(
                new Vector3(0, 0, 0),
                new Vector3(0, 0, 0),
                new Vector3(2, 0, 0)
                ),
                new Path.PathPoint(
                new Vector3(8, 4, 2),
                new Vector3(-4, 0, 4),
                new Vector3(4, 0, -4)
                ),
                new Path.PathPoint(
                new Vector3(4, 2, -6),
                new Vector3(0, 0, 0),
                new Vector3(0, 0, 0)
                )
            };
            scene.objects.Add(new Path(pathPoints));

            listBox1.Items.Add("path");
            pathPoints = new List<Path.PathPoint>
            {
                new Path.PathPoint(
                new Vector3(-3, 6, 0),
                new Vector3(0, 0, -1.75f),
                new Vector3(0, 0, 1.75f)
                ),
                new Path.PathPoint(
                new Vector3(0, 6, 3),
                new Vector3(-1.75f, 0, 0),
                new Vector3(1.75f, 0, 0)
                ),
                new Path.PathPoint(
                new Vector3(3, 6, 0),
                new Vector3(0, 0, 1.75f),
                new Vector3(0, 0, -1.75f)
                ),
                new Path.PathPoint(
                new Vector3(0, 6, -3),
                new Vector3(1.75f, 0, 0),
                new Vector3(-1.75f, 0, 0)
                )
            };
            scene.objects.Add(new Path(pathPoints) { Closed = true });

            listBox1.Items.Add("path");
            /*pathPoints = new List<Path.PathPoint>();
            pathPoints.Add(new Path.PathPoint(
                new Vector3(-3, 6, 0),
                new Vector3(0, 0, -1.75f),
                new Vector3(0, 0, 1.75f)
                ));
            pathPoints.Add(new Path.PathPoint(
                new Vector3(0, 6, 3),
                new Vector3(-1.75f, 0, 0),
                new Vector3(1.75f, 0, 0)
                ));
            pathPoints.Add(new Path.PathPoint(
                new Vector3(3, 6, 0),
                new Vector3(0, 0, 1.75f),
                new Vector3(0, 0, -1.75f)
                ));
            pathPoints.Add(new Path.PathPoint(
                new Vector3(0, 6, -3),
                new Vector3(1.75f, 0, 0),
                new Vector3(-1.75f, 0, 0)
                ));*/
            scene.objects.Add(new Path(pathPoints) { Closed = true });

            for (int i = 5; i<10000; i++)
            {
                listBox1.Items.Add("block");
                scene.objects.Add(new TransformableObject(new Vector3(i,0,0)));
            }

            gL_ControlModern1.MainDrawable = scene;
            gL_ControlModern1.ActiveCamera = new GL_EditorFramework.StandardCameras.InspectCamera(1f);

            gL_ControlLegacy1.MainDrawable = new SingleObject(new Vector3());
            gL_ControlModern1.CameraDistance = 20;
            scene.SelectionChanged += Scene_SelectionChanged;
            scene.ObjectsMoved += Scene_ObjectsMoved;
            listBox1.SelectedIndexChanged += ListBox1_SelectedIndexChanged;
            objectPropertyControl1.ValueChanged += ObjectPropertyControl1_ValueChanged;
            objectPropertyControl1.ValueSet += ObjectPropertyControl1_ValueChanged;
            objectPropertyControl1.ValueChangeStart += ObjectPropertyControl1_ValueChangeStart;
            objectPropertyControl1.ValueSet += ObjectPropertyControl1_ValueSet;
            gL_ControlModern1.KeyDown += GL_ControlModern1_KeyDown;
        }

        private void GL_ControlModern1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                scene.Delete(scene.SelectedObjects.ToArray());
        }

        private void ObjectPropertyControl1_ValueSet(object sender, EventArgs e)
        {
            scene.ApplyCurrentTransformAction();
            propertyContainer.pCenter = propertyContainer.center;
        }

        private void ObjectPropertyControl1_ValueChangeStart(object sender, EventArgs e)
        {
            scene.CurrentAction = propertyChangesAction;
        }

        private void Scene_ObjectsMoved(object sender, EventArgs e)
        {
            propertyContainer.Setup(scene.SelectedObjects);
            objectPropertyControl1.Refresh();
        }

        private void ObjectPropertyControl1_ValueChanged(object sender, EventArgs e)
        {
            propertyChangesAction.translation = propertyContainer.center - propertyContainer.pCenter;

            gL_ControlModern1.Refresh();
        }

        private void Scene_SelectionChanged(object sender, EventArgs e)
        {
            listBox1.SelectedIndexChanged -= ListBox1_SelectedIndexChanged;
            listBox1.SelectedIndices.Clear();
            int i = 0;
            foreach(EditableObject o in scene.objects)
            {
                if(o.IsSelected())
                    listBox1.SelectedIndices.Add(i);
                i++;
            }

            listBox1.SelectedIndexChanged += ListBox1_SelectedIndexChanged;

            if (listBox1.SelectedIndices.Count > 0)
            {
                propertyContainer.Setup(scene.SelectedObjects);

                objectPropertyControl1.CurrentPropertyContainer = propertyContainer;
            }
            else
            {
                if (objectPropertyControl1.CurrentPropertyContainer != null)
                    objectPropertyControl1.CurrentPropertyContainer = null;
            }
        }

        private void ListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            List<EditableObject> newSelection = new List<EditableObject>();
            foreach(int i in listBox1.SelectedIndices)
            {
                newSelection.Add(scene.objects[i]);
            }

            scene.SelectedObjects = newSelection;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            listBox1.Items.Add("block"); //make sure to add the entry before you add an object because the SelectionChanged event will be fired
            scene.Add(new TransformableObject(new Vector3()));
        }
    }

    public class TestContainer : AbstractPropertyContainer
    {
        public Vector3 pCenter;
        public Vector3 center;

        public void Setup(List<EditableObject> editableObjects)
        {
            EditableObject.BoundingBox box = EditableObject.BoundingBox.Default;
            foreach (EditableObject obj in editableObjects)
            {
                box.Include(obj.GetSelectionBox());
            }
            center = box.GetCenter();
            pCenter = center;
        }

        public override void DoUI(IObjectPropertyControl control)
        {
            center.X = control.NumberInput(center.X, "x", 0.125f);
            center.Y = control.NumberInput(center.Y, "y", 0.125f);
            center.Z = control.NumberInput(center.Z, "z", 0.125f);
        }
    }
}

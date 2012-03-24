﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace Vaerydian.Utils
{
    public class QuadNode<E> where E : class
    {
        private Vector2 q_ULCorner;
        /// <summary>
        /// represents UL corner
        /// </summary>
        public Vector2 ULCorner
        {
            get { return q_ULCorner; }
            set { q_ULCorner = value; }
        }
        /// <summary>
        /// represents LR corner
        /// </summary>
        private Vector2 q_LRCorner;

        public Vector2 LRCorner
        {
            get { return q_LRCorner; }
            set { q_LRCorner = value; }
        }
        

        private QuadNode<E> q_Q1;

        /// <summary>
        /// quadrent 1
        /// </summary>
        public QuadNode<E> Q1
        {
            get { return q_Q1; }
            set { q_Q1 = value; }
        }
        
        private QuadNode<E> q_Q2;
        /// <summary>
        /// quadrent 2
        /// </summary>
        public QuadNode<E> Q2
        {
            get { return q_Q2; }
            set { q_Q2 = value; }
        }
        private QuadNode<E> q_Q3;
        /// <summary>
        /// quadrent 3
        /// </summary>
        public QuadNode<E> Q3
        {
            get { return q_Q3; }
            set { q_Q3 = value; }
        }
        private QuadNode<E> q_Q4;

        /// <summary>
        /// quadrent 4
        /// </summary>
        public QuadNode<E> Q4
        {
            get { return q_Q4; }
            set { q_Q4 = value; }
        }
        private List<E> q_Contents;

        /// <summary>
        /// contents of node
        /// </summary>
        public List<E> Contents
        {
            get { return q_Contents; }
            set { q_Contents = value; }
        }

        private QuadNode<E> q_Parent;

        /// <summary>
        /// parent node
        /// </summary>
        public QuadNode<E> Parent
        {
            get { return q_Parent; }
            set { q_Parent = value; }
        }

        public QuadNode()
        {
            q_Contents = new List<E>();
        }

        public QuadNode(Vector2 ul, Vector2 lr)
        {
            q_ULCorner = ul;
            q_LRCorner = lr;
            q_Contents = new List<E>();
        }

        /// <summary>
        /// does node contain current point
        /// </summary>
        /// <param name="point">point to check</param>
        /// <returns>true if it does</returns>
        public bool contains(Vector2 point)
        {
            if ((point.X < q_LRCorner.X) &&
                (point.X >= q_ULCorner.X) &&
                (point.Y < q_LRCorner.Y) &&
                (point.Y >= q_ULCorner.Y))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// subdivides node CW into 4 new nodes
        /// </summary>
        public void subdivide()
        {
            float midX = q_ULCorner.X + (q_LRCorner.X - q_ULCorner.X) / 2.0f;
            float midY = q_ULCorner.Y + (q_LRCorner.Y - q_ULCorner.Y) / 2.0f;
            q_Q1 = new QuadNode<E>(q_ULCorner, new Vector2(midX, midY));
            q_Q1.Parent = this;
            q_Q2 = new QuadNode<E>(new Vector2(midX, q_ULCorner.Y), new Vector2(q_LRCorner.X, midY));
            q_Q2.Parent = this;
            q_Q3 = new QuadNode<E>(new Vector2(q_ULCorner.X, midY), new Vector2(midX, q_LRCorner.Y));
            q_Q3.Parent = this;
            q_Q4 = new QuadNode<E>(new Vector2(midX, midY), q_LRCorner);
            q_Q4.Parent = this;
        }

    }
}
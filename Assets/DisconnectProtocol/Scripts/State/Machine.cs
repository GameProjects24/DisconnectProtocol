using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

namespace State {
    public class State : MonoBehaviour {
        public virtual void Act() {}
        public virtual void Enter() {}
        /// <summary>
        /// Only explicit Exit through this method allowed. State cannot Exit by itself
        /// </summary>
        public virtual void Exit() {}
    }

    public class Condition : MonoBehaviour {
        public virtual bool Eval() => false;
    }

    [Serializable]
    class Transition {
        public Condition condition;
        public State state;

        internal bool IsPossible() {
            return condition.Eval();
        }
    }

    [Serializable]
    class Node {
        public State state;
        public List<Transition> transitions;
    }

    public class Machine : MonoBehaviour {
        [SerializeField] private Node m_entry;
        [SerializeField] private List<Node> m_nodes;
        private Node m_current = null;

        public void Execute() {
            if (m_current == null) {
                m_current = m_entry;
            }
            foreach (var t in m_current.transitions) {
                if (t.IsPossible()) {
                    m_current.state.Exit();
                    m_current = NodeByState(t.state);
                    m_current.state.Enter();
                    break;
                }
            }
            m_current.state.Act();
        }

        private Node NodeByState(State state) {
            foreach (var n in m_nodes) {
                if (n.state == state) {
                    return n;
                }
            }
            return m_entry;
        }
    }
}
using System;
using UnityEngine;
using UnityEngine.AI;

namespace DisconnectProtocol {
    public abstract class EnemyState {
        public NavMeshAgent body;
        public EnemyState(NavMeshAgent b) {
            body = b;
        }
        public event System.Action OnExit;
        public abstract bool CanEnter();
        public abstract void Enter();
        public abstract void Update();
        public virtual void Exit() {
            OnExit?.Invoke();
        }
    }

    public class Idle: EnemyState {
        public Idle(NavMeshAgent b): base(b) {}

        public override bool CanEnter() => true;
        public override void Enter() {
            body.isStopped = true;
        }
        public override void Update() { }
    }

    public class HoldDistance: EnemyState {
        Transform player;
        float speed = 10f;
        float distance = 7f;
        float angleVision = 30f;
        float distVision = 15f;

        bool isPlayerSeen() {
            var dir = player.position - body.transform.position;
            return dir.sqrMagnitude < distVision * distVision
                && Vector3.Angle(dir, body.transform.forward) < angleVision;
        }

        public HoldDistance(NavMeshAgent b, Transform p, float av, float dv, float sp): base(b) {
            player = p;
            angleVision = av;
            distVision = dv;
            speed = sp;
        }

        public override bool CanEnter() {
            return isPlayerSeen();
        }

        public override void Enter() {
            body.stoppingDistance = 2f;
            body.speed = speed;
            body.isStopped = false;
        }

        public override void Update() {
            if (!isPlayerSeen()) {
                Exit();
                return;
            }
            var dir = player.position - body.transform.position;
            var point = (distance - dir.magnitude)*dir.normalized;
            body.SetDestination(player.position);
            if (dir.magnitude < distance) {
                body.isStopped = true;
                body.ResetPath();
            }
        }

        public override void Exit() {
            body.isStopped = true;
            body.ResetPath();
            base.Exit();
        }
    }
}
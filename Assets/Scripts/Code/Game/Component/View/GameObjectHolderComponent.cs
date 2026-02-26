using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TaoTie
{
    public partial class GameObjectHolderComponent : Component, IComponent
    {
        public Transform EntityView;
        private ReferenceCollector collector;
        public Animator Animator;
        private Queue<ETTask> waitFinishTask;
        #region override
        
        public void Init()
        {
            LoadGameObjectAsync().Coroutine();
        }
        
        private async ETTask LoadGameObjectAsync()
        {
            var unit = this.GetParent<Unit>();
            GameObject obj = await GameObjectPoolManager.GetInstance().GetGameObjectAsync(unit.Config.Perfab);
            
            if (this.IsDispose)
            {
                GameObjectPoolManager.GetInstance().RecycleGameObject(obj);
                return;
            }

            EntityView = obj.transform;
            collector = obj.GetComponent<ReferenceCollector>();
            Animator = obj.GetComponent<Animator>();
            if (Animator == null)
            {
                Animator = obj.GetComponentInChildren<Animator>();
            }
            EntityView.SetParent(this.parent.Parent.GameObjectRoot);
            var ec = obj.GetComponent<EntityComponent>();
            if (ec == null) ec = obj.AddComponent<EntityComponent>();
            ec.Id = this.Id;
            ec.EntityType = parent.Type;

            EntityView.position = unit.Position;
            EntityView.rotation = unit.Rotation;
            EntityView.localScale = unit.LocalScale;
            Messager.Instance.AddListener<Unit, Vector3>(Id, MessageId.ChangePositionEvt, OnChangePosition);
            Messager.Instance.AddListener<Unit, Quaternion>(Id, MessageId.ChangeRotationEvt, OnChangeRotation);
            Messager.Instance.AddListener<Unit, Vector3>(Id, MessageId.ChangeScaleEvt, OnChangeScale);
            
           
            if (Animator != null && !string.IsNullOrEmpty(unit.Config.Controller))
            {
                var controller = await ResourcesManager.Instance.LoadAsync<RuntimeAnimatorController>(unit.Config.Controller);
                if (controller != null)
                {
                    Animator.runtimeAnimatorController = controller;
                }
            }
            
            if (waitFinishTask != null)
            {
                while (waitFinishTask.TryDequeue(out var task))
                {
                    task.SetResult();
                }

                waitFinishTask = null;
            }
            
        }

        public void Destroy()
        {
            
            Messager.Instance.RemoveListener<Unit, Vector3>(Id, MessageId.ChangePositionEvt, OnChangePosition);
            Messager.Instance.RemoveListener<Unit, Quaternion>(Id, MessageId.ChangeRotationEvt, OnChangeRotation);
            Messager.Instance.RemoveListener<Unit, Vector3>(Id, MessageId.ChangeScaleEvt, OnChangeScale);
            
            
            if (EntityView != null)
            {
                if (Animator != null && Animator.runtimeAnimatorController != null)
                {
                    ResourcesManager.Instance.ReleaseAsset(Animator.runtimeAnimatorController);
                    Animator.runtimeAnimatorController = null;
                }
                Animator = null;
                GameObjectPoolManager.GetInstance().RecycleGameObject(EntityView.gameObject);

                EntityView = null;
                collector = null;
            }

            if (waitFinishTask != null)
            {
                while (waitFinishTask.TryDequeue(out var task))
                {
                    task.SetResult();
                }
                waitFinishTask = null;
            }
            
        }

        #endregion

        #region Event
        
        private void OnChangePosition(Unit unit, Vector3 old)
        {
            if(EntityView == null) return;
            EntityView.position = unit.Position;
        }

        private void OnChangeRotation(Unit unit, Quaternion old)
        {
            if(EntityView == null) return;
            EntityView.rotation = unit.Rotation;
        }
        private void OnChangeScale(Unit unit, Vector3 old)
        {
            if(EntityView == null) return;
            EntityView.localScale = unit.LocalScale;
        }

        #endregion
        /// <summary>
        /// 等待预制体加载完成，注意判断加载完之后Component是否已经销毁
        /// </summary>
        public async ETTask<bool> WaitLoadGameObjectOver()
        {
            if (EntityView == null)
            {
                ETTask task = ETTask.Create(true);
                if (waitFinishTask == null)
                    waitFinishTask = new Queue<ETTask>();
                waitFinishTask.Enqueue(task);
                await task;
            }
            return !IsDispose;
        }

        public T GetCollectorObj<T>(string name) where T : class
        {
            if (collector == null) return null;
            return collector.Get<T>(name);
        }
        
        /// <summary>
        /// 开启或关闭Renderer
        /// </summary>
        /// <param name="enable"></param>
        public async ETTask EnableRenderer(bool enable)
        {
            CoroutineLock coroutineLock = null;
            try
            {
                coroutineLock = await CoroutineLockManager.Instance.Wait(CoroutineLockType.EnableObjView, parent.Id);
                await this.WaitLoadGameObjectOver();
                if(this.IsDispose) return;
                var renders = this.EntityView.GetComponentsInChildren<Renderer>(true);
                for (int i = 0; i < renders.Length; i++)
                {
                    //if(renders[i] is ParticleSystemRenderer) continue;
                    renders[i].enabled = enable;
                }
            }
            finally
            {
                coroutineLock?.Dispose();
            }
            
        }
        
        /// <summary>
        /// 开启或关闭hitBox
        /// </summary>
        /// <param name="hitBox"></param>
        /// <param name="enable"></param>
        public async ETTask EnableHitBox(string hitBox, bool enable)
        {
            await this.WaitLoadGameObjectOver();
            if(this.IsDispose) return;
            this.GetCollectorObj<GameObject>(hitBox)?.SetActive(enable);
        }
    }
}
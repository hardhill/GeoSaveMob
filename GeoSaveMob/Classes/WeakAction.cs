using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LCadLau2.Classes
{
    public class WeakAction
    {
        private Action _staticAction;

        //
        // Сводка:
        //     Gets or sets the System.Reflection.MethodInfo corresponding to this WeakAction's
        //     method passed in the constructor.
        protected MethodInfo Method { get; set; }

        //
        // Сводка:
        //     Gets the name of the method that this WeakAction represents.
        public virtual string MethodName
        {
            get
            {
                if (_staticAction != null)
                {
                    return _staticAction.GetMethodInfo().Name;
                }

                return Method.Name;
            }
        }

        //
        // Сводка:
        //     Gets or sets a WeakReference to this WeakAction's action's target. This is not
        //     necessarily the same as GalaSoft.MvvmLight.Helpers.WeakAction.Reference, for
        //     example if the method is anonymous.
        protected WeakReference ActionReference { get; set; }

        //
        // Сводка:
        //     Saves the GalaSoft.MvvmLight.Helpers.WeakAction.ActionReference as a hard reference.
        //     This is used in relation with this instance's constructor and only if the constructor's
        //     keepTargetAlive parameter is true.
        protected object LiveReference { get; set; }

        //
        // Сводка:
        //     Gets or sets a WeakReference to the target passed when constructing the WeakAction.
        //     This is not necessarily the same as GalaSoft.MvvmLight.Helpers.WeakAction.ActionReference,
        //     for example if the method is anonymous.
        protected WeakReference Reference { get; set; }

        //
        // Сводка:
        //     Gets a value indicating whether the WeakAction is static or not.
        public bool IsStatic => _staticAction != null;

        //
        // Сводка:
        //     Gets a value indicating whether the Action's owner is still alive, or if it was
        //     collected by the Garbage Collector already.
        public virtual bool IsAlive
        {
            get
            {
                if (_staticAction == null && Reference == null && LiveReference == null)
                {
                    return false;
                }

                if (_staticAction != null)
                {
                    if (Reference != null)
                    {
                        return Reference.IsAlive;
                    }

                    return true;
                }

                if (LiveReference != null)
                {
                    return true;
                }

                if (Reference != null)
                {
                    return Reference.IsAlive;
                }

                return false;
            }
        }

        //
        // Сводка:
        //     Gets the Action's owner. This object is stored as a System.WeakReference.
        public object Target
        {
            get
            {
                if (Reference == null)
                {
                    return null;
                }

                return Reference.Target;
            }
        }

        //
        // Сводка:
        //     The target of the weak reference.
        protected object ActionTarget
        {
            get
            {
                if (LiveReference != null)
                {
                    return LiveReference;
                }

                if (ActionReference == null)
                {
                    return null;
                }

                return ActionReference.Target;
            }
        }

        //
        // Сводка:
        //     Initializes an empty instance of the GalaSoft.MvvmLight.Helpers.WeakAction class.
        protected WeakAction()
        {
        }

        //
        // Сводка:
        //     Initializes a new instance of the GalaSoft.MvvmLight.Helpers.WeakAction class.
        //
        // Параметры:
        //   action:
        //     The action that will be associated to this instance.
        //
        //   keepTargetAlive:
        //     If true, the target of the Action will be kept as a hard reference, which might
        //     cause a memory leak. You should only set this parameter to true if the action
        //     is using closures. See http://galasoft.ch/s/mvvmweakaction.
        public WeakAction(Action action, bool keepTargetAlive = false)
            : this(action?.Target, action, keepTargetAlive)
        {
        }

        //
        // Сводка:
        //     Initializes a new instance of the GalaSoft.MvvmLight.Helpers.WeakAction class.
        //
        // Параметры:
        //   target:
        //     The action's owner.
        //
        //   action:
        //     The action that will be associated to this instance.
        //
        //   keepTargetAlive:
        //     If true, the target of the Action will be kept as a hard reference, which might
        //     cause a memory leak. You should only set this parameter to true if the action
        //     is using closures. See http://galasoft.ch/s/mvvmweakaction.
        public WeakAction(object target, Action action, bool keepTargetAlive = false)
        {
            if (action.GetMethodInfo().IsStatic)
            {
                _staticAction = action;
                if (target != null)
                {
                    Reference = new WeakReference(target);
                }
            }
            else
            {
                Method = action.GetMethodInfo();
                ActionReference = new WeakReference(action.Target);
                LiveReference = (keepTargetAlive ? action.Target : null);
                Reference = new WeakReference(target);
            }
        }

        //
        // Сводка:
        //     Executes the action. This only happens if the action's owner is still alive.
        public void Execute()
        {
            if (_staticAction != null)
            {
                _staticAction();
                return;
            }

            object actionTarget = ActionTarget;
            if (IsAlive && (object)Method != null && (LiveReference != null || ActionReference != null) && actionTarget != null)
            {
                Method.Invoke(actionTarget, null);
            }
        }

        //
        // Сводка:
        //     Sets the reference that this instance stores to null.
        public void MarkForDeletion()
        {
            Reference = null;
            ActionReference = null;
            LiveReference = null;
            Method = null;
            _staticAction = null;
        }
    }
}

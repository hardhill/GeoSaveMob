using System;
using System.Reflection;

namespace LCadLau2.Classes
{
    public class WeakFunc<TResult>
    {
        private Func<TResult> _staticFunc;

        //
        // Сводка:
        //     Gets or sets the System.Reflection.MethodInfo corresponding to this WeakFunc's
        //     method passed in the constructor.
        protected MethodInfo Method { get; set; }

        //
        // Сводка:
        //     Get a value indicating whether the WeakFunc is static or not.
        public bool IsStatic => _staticFunc != null;

        //
        // Сводка:
        //     Gets the name of the method that this WeakFunc represents.
        public virtual string MethodName
        {
            get
            {
                if (_staticFunc != null)
                {
                    return _staticFunc.GetMethodInfo().Name;
                }

                return Method.Name;
            }
        }

        //
        // Сводка:
        //     Gets or sets a WeakReference to this WeakFunc's action's target. This is not
        //     necessarily the same as GalaSoft.MvvmLight.Helpers.WeakFunc`1.Reference, for
        //     example if the method is anonymous.
        protected WeakReference FuncReference { get; set; }

        //
        // Сводка:
        //     Saves the GalaSoft.MvvmLight.Helpers.WeakFunc`1.FuncReference as a hard reference.
        //     This is used in relation with this instance's constructor and only if the constructor's
        //     keepTargetAlive parameter is true.
        protected object LiveReference { get; set; }

        //
        // Сводка:
        //     Gets or sets a WeakReference to the target passed when constructing the WeakFunc.
        //     This is not necessarily the same as GalaSoft.MvvmLight.Helpers.WeakFunc`1.FuncReference,
        //     for example if the method is anonymous.
        protected WeakReference Reference { get; set; }

        //
        // Сводка:
        //     Gets a value indicating whether the Func's owner is still alive, or if it was
        //     collected by the Garbage Collector already.
        public virtual bool IsAlive
        {
            get
            {
                if (_staticFunc == null && Reference == null && LiveReference == null)
                {
                    return false;
                }

                if (_staticFunc != null)
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
        //     Gets the Func's owner. This object is stored as a System.WeakReference.
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
        //     Gets the owner of the Func that was passed as parameter. This is not necessarily
        //     the same as GalaSoft.MvvmLight.Helpers.WeakFunc`1.Target, for example if the
        //     method is anonymous.
        protected object FuncTarget
        {
            get
            {
                if (LiveReference != null)
                {
                    return LiveReference;
                }

                if (FuncReference == null)
                {
                    return null;
                }

                return FuncReference.Target;
            }
        }

        //
        // Сводка:
        //     Initializes an empty instance of the WeakFunc class.
        protected WeakFunc()
        {
        }

        //
        // Сводка:
        //     Initializes a new instance of the WeakFunc class.
        //
        // Параметры:
        //   func:
        //     The Func that will be associated to this instance.
        //
        //   keepTargetAlive:
        //     If true, the target of the Action will be kept as a hard reference, which might
        //     cause a memory leak. You should only set this parameter to true if the action
        //     is using closures. See http://galasoft.ch/s/mvvmweakaction.
        public WeakFunc(Func<TResult> func, bool keepTargetAlive = false)
            : this(func?.Target, func, keepTargetAlive)
        {
        }

        //
        // Сводка:
        //     Initializes a new instance of the WeakFunc class.
        //
        // Параметры:
        //   target:
        //     The Func's owner.
        //
        //   func:
        //     The Func that will be associated to this instance.
        //
        //   keepTargetAlive:
        //     If true, the target of the Action will be kept as a hard reference, which might
        //     cause a memory leak. You should only set this parameter to true if the action
        //     is using closures. See http://galasoft.ch/s/mvvmweakaction.
        public WeakFunc(object target, Func<TResult> func, bool keepTargetAlive = false)
        {
            if (func.GetMethodInfo().IsStatic)
            {
                _staticFunc = func;
                if (target != null)
                {
                    Reference = new WeakReference(target);
                }
            }
            else
            {
                Method = func.GetMethodInfo();
                FuncReference = new WeakReference(func.Target);
                LiveReference = (keepTargetAlive ? func.Target : null);
                Reference = new WeakReference(target);
            }
        }

        //
        // Сводка:
        //     Executes the action. This only happens if the Func's owner is still alive.
        //
        // Возврат:
        //     The result of the Func stored as reference.
        public TResult Execute()
        {
            if (_staticFunc != null)
            {
                return _staticFunc();
            }

            object funcTarget = FuncTarget;
            if (IsAlive && (object)Method != null && (LiveReference != null || FuncReference != null) && funcTarget != null)
            {
                return (TResult)Method.Invoke(funcTarget, null);
            }

            return default(TResult);
        }

        //
        // Сводка:
        //     Sets the reference that this instance stores to null.
        public void MarkForDeletion()
        {
            Reference = null;
            FuncReference = null;
            LiveReference = null;
            Method = null;
            _staticFunc = null;
        }
    }
}

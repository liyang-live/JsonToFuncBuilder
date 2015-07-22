using System;
using System.Linq;
using System.Reflection;
using Fixie;
using Ploeh.AutoFixture.AutoNSubstitute;

namespace JsonToFuncBuilderTests.Tests
{
    public class TestConvention : Convention
    {
        public TestConvention()
        {
            Classes.NameEndsWith("Test");
            ClassExecution.CreateInstancePerCase();

            Methods.Where(x => x.IsVoid() && x.IsPublic);

            FixtureExecution.Wrap<AMFixtureBehavior>();
        }
    }

    public class AMFixtureBehavior : FixtureBehavior
    {
        public void Execute(Fixture context, Action next)
        {
            var registry = new Ploeh.AutoFixture.Fixture().Customize(new AutoNSubstituteCustomization());

            Func<int> randomValue = () =>
            {
                var rand = new Random((int)DateTime.Now.Ticks);
                return rand.Next(40, 100);
            };

            registry.Customize<Customer>(x => x.With(p => p.Age, randomValue.Invoke()));

            context.TryField("Registry", registry);
            
            next();
        }
    }

    public static class BehaviorBuilderExtensions
    {
        public static void TryField(this Fixture context, string fieldName, object fieldValue)
        {
            var lifecycleMethod = context.Class.Type.GetField(fieldName, BindingFlags.Public | BindingFlags.Instance);

            if (lifecycleMethod == null)
                return;

            try
            {
                lifecycleMethod.SetValue(context.Instance, fieldValue);
            }
            catch (TargetInvocationException exception)
            {
                throw new PreservedException(exception.InnerException);
            }
        }


        public static void TryInvoke(this Type type, string method, object instance)
        {
            var lifecycleMethod =
                type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .SingleOrDefault(x => ReflectionExtensions.HasSignature(x, typeof (void), method));

            if (lifecycleMethod == null)
                return;

            try
            {
                lifecycleMethod.Invoke(instance, null);
            }
            catch (TargetInvocationException exception)
            {
                throw new PreservedException(exception.InnerException);
            }
        }
    }
}
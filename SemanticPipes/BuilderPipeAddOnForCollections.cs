using System;
using System.Collections;
using System.Collections.Generic;

namespace SemanticPipes
{
    public sealed class BuilderPipeAddOnForCollections
    {
        public static void InstallHandler(object sender, SemanticPipeInstalledEventArgs args)
        {
            Func<Type, Type> generateTargetType = type => type.MakeGenericType(args.PipeExtension.DestinationType);

            Type listDestinationType = generateTargetType(typeof (List<>));

            Func<object, object> processCallbackFunc = o =>
            {
                var list = (IList) Activator.CreateInstance(listDestinationType);
                list.Add(args.PipeExtension.ProcessCallback(o));
                return list;
            };
            Action<Type> registerTarget = type =>
            {
                var pipeOutputPackage = new PipeOutputPackage(args.PipeExtension.SourceType, type, processCallbackFunc);
                args.AppendPackage(pipeOutputPackage);
            };

            registerTarget(listDestinationType);
            registerTarget(generateTargetType(typeof (IEnumerable<>)));
            registerTarget(generateTargetType(typeof (IList<>)));
            registerTarget(generateTargetType(typeof (ICollection<>)));
        }
    }
}
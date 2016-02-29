﻿using DotVVM.Framework.Runtime.Compilation.Binding;
using DotVVM.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DotVVM.Framework.Runtime
{
    public class BindingParserOptions
    {
        public Type BindingType { get; }
        public string ScopeParameter { get; }

        public string[] ImportNamespaces { get; set; } = new string[0];

        public virtual TypeRegistry AddTypes(TypeRegistry reg) => reg.AddSymbols(ImportNamespaces.Select(n =>
            (Func<string, Expression>)(t => TypeRegistry.CreateStatic(ReflectionUtils.FindType(n + "." + t)))));

        public BindingParserOptions(Type bindingType, string scopeParameter = "_this")
        {
            BindingType = bindingType;
            ScopeParameter = scopeParameter;
        }

        public static BindingParserOptions Create<TBinding>(string scopeParameter = "_this")
            => new BindingParserOptions(typeof(TBinding), scopeParameter);
    }
}

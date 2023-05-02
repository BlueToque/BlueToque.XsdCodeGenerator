//=============================================================================
//
// Copyright (C) 2007 Michael Coyle, Blue Toque
// Copyright (C) 2023 Michael Coyle, Blue Toque Software
// http://www.BlueToque.ca/Products/CodeGeneration.html
// michael.coyle@BlueToque.ca
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// http://www.gnu.org/licenses/gpl.txt
//
//=============================================================================
using CodeGeneration.Internal;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Xml;

namespace CodeGeneration
{
    /// <summary>
    /// This class is a specialization of the MethodCall class that
    /// uses a special method to generate code that is more efficient (up to 10 
    /// times faster) than using reflection to call a method.
    /// 
    /// It is experimental but it is included in this code to demonstrate that
    /// we can specialise  our method calling for different situations.
    /// </summary>
    public class FastMethodCall : MethodCall
    {
        /// <summary>
        /// Assumptions: 
        ///     assembly that methodInfo is in has been loaded
        ///     the class the methodinfo is in has a parameterless constructor        
        /// </summary>
        /// <param name="methodInfo"></param>
        public FastMethodCall(MethodInfo methodInfo, string identifier)
            : base(methodInfo, identifier) => m_fastInvokeHandler = GetMethodInvoker(m_methodInfo);

        /// <summary>
        /// Copy constructor, used by the aggregate web method node
        /// </summary>
        /// <param name="methodCall"></param>
        public FastMethodCall(IMethodCall methodCall)
            : base(methodCall) => m_fastInvokeHandler = GetMethodInvoker(m_methodInfo);

        readonly FastInvokeHandler m_fastInvokeHandler;

        /// <summary>
        /// Take an XMLDocument that conforms to the input schema and call the method
        /// return and XML Document that conforms to the output schema.
        /// This override uses the FastMethodInvoker described at 
        /// http://www.codeproject.com/csharp/FastMethodInvoker.asp
        /// (this can invoke the method using reflection.emit up to 10 times faster
        /// than by reflection, and only 4 times slower than by invoking the method 
        /// directly)
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public override XmlDocument Execute(XmlDocument document)
        {
            // assign the XML data to the parameter objects
            object[] parameterObjects = MethodCallTools.XmlToMethodInput(m_methodInfo, m_inputType, document);

            // invoke the method
            object returnObject = m_fastInvokeHandler( m_object, parameterObjects);

            List<object> returnObjects = new List<object>();
            if (parameterObjects != null)
                returnObjects.AddRange(parameterObjects);
            returnObjects.Add(returnObject);

            m_returnObject = MethodCallTools.MethodOutputToObject(m_methodInfo, returnObjects, m_outputType);

            //Type type = m_outputType.IsByRef ? m_outputType.GetElementType() : m_outputType;
            string results = Serializer.Serialize(m_returnObject);

            // make sure to create a NEW document here becuase otherwise callers from different
            // threads will get a modified document
            document = new XmlDocument();
            document.LoadXml(results);
            return document;

        }

        #region utility

        delegate object FastInvokeHandler(object target, object[] paramters);

        private static FastInvokeHandler GetMethodInvoker(MethodInfo methodInfo)
        {
            DynamicMethod dynamicMethod = new DynamicMethod(string.Empty, typeof(object), new Type[] { typeof(object), typeof(object[]) }, methodInfo.DeclaringType.Module);
            ILGenerator il = dynamicMethod.GetILGenerator();
            ParameterInfo[] ps = methodInfo.GetParameters();
            Type[] paramTypes = new Type[ps.Length];
            for (int i = 0; i < paramTypes.Length; i++)
                paramTypes[i] = ps[i].ParameterType.IsByRef ? ps[i].ParameterType.GetElementType() : ps[i].ParameterType;

            LocalBuilder[] locals = new LocalBuilder[paramTypes.Length];

            for (int i = 0; i < paramTypes.Length; i++)
                locals[i] = il.DeclareLocal(paramTypes[i], true);

            for (int i = 0; i < paramTypes.Length; i++)
            {
                il.Emit(OpCodes.Ldarg_1);
                EmitFastInt(il, i);
                il.Emit(OpCodes.Ldelem_Ref);
                EmitCastToReference(il, paramTypes[i]);
                il.Emit(OpCodes.Stloc, locals[i]);
            }

            if (!methodInfo.IsStatic)
                il.Emit(OpCodes.Ldarg_0);

            for (int i = 0; i < paramTypes.Length; i++)
            {
                if (ps[i].ParameterType.IsByRef)
                    il.Emit(OpCodes.Ldloca_S, locals[i]);
                else
                    il.Emit(OpCodes.Ldloc, locals[i]);
            }

            if (methodInfo.IsStatic)
                il.EmitCall(OpCodes.Call, methodInfo, null);
            else
                il.EmitCall(OpCodes.Callvirt, methodInfo, null);

            if (methodInfo.ReturnType == typeof(void))
                il.Emit(OpCodes.Ldnull);
            else
                EmitBoxIfNeeded(il, methodInfo.ReturnType);

            for (int i = 0; i < paramTypes.Length; i++)
            {
                if (ps[i].ParameterType.IsByRef)
                {
                    il.Emit(OpCodes.Ldarg_1);
                    EmitFastInt(il, i);
                    il.Emit(OpCodes.Ldloc, locals[i]);
                    if (locals[i].LocalType.IsValueType)
                        il.Emit(OpCodes.Box, locals[i].LocalType);
                    il.Emit(OpCodes.Stelem_Ref);
                }
            }

            il.Emit(OpCodes.Ret);
            FastInvokeHandler invoder = (FastInvokeHandler)dynamicMethod.CreateDelegate(typeof(FastInvokeHandler));
            return invoder;
        }

        private static void EmitCastToReference(ILGenerator il, System.Type type)
        {
            if (type.IsValueType)
                il.Emit(OpCodes.Unbox_Any, type);
            else
                il.Emit(OpCodes.Castclass, type);
        }

        private static void EmitBoxIfNeeded(ILGenerator il, System.Type type)
        {
            if (type.IsValueType)
                il.Emit(OpCodes.Box, type);
        }

        private static void EmitFastInt(ILGenerator il, int value)
        {
            switch (value)
            {
                case -1: il.Emit(OpCodes.Ldc_I4_M1); return;
                case 0: il.Emit(OpCodes.Ldc_I4_0); return;
                case 1: il.Emit(OpCodes.Ldc_I4_1); return;
                case 2: il.Emit(OpCodes.Ldc_I4_2); return;
                case 3: il.Emit(OpCodes.Ldc_I4_3); return;
                case 4: il.Emit(OpCodes.Ldc_I4_4); return;
                case 5: il.Emit(OpCodes.Ldc_I4_5); return;
                case 6: il.Emit(OpCodes.Ldc_I4_6); return;
                case 7: il.Emit(OpCodes.Ldc_I4_7); return;
                case 8: il.Emit(OpCodes.Ldc_I4_8); return;
            }

            if (value > -129 && value < 128)
                il.Emit(OpCodes.Ldc_I4_S, (SByte)value);
            else
                il.Emit(OpCodes.Ldc_I4, value);
        }
        #endregion

    }
}

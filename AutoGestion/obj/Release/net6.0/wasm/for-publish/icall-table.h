#define ICALL_TABLE_corlib 1

static int corlib_icall_indexes [] = {
187,
192,
193,
194,
195,
196,
197,
198,
199,
200,
203,
204,
321,
322,
324,
353,
354,
355,
375,
376,
377,
378,
379,
470,
471,
472,
475,
514,
515,
517,
519,
521,
523,
528,
536,
537,
538,
539,
540,
541,
542,
543,
544,
545,
546,
688,
689,
696,
699,
701,
706,
707,
709,
710,
714,
715,
716,
717,
719,
720,
721,
724,
725,
728,
729,
730,
798,
799,
801,
809,
810,
811,
812,
813,
817,
818,
819,
820,
821,
822,
824,
825,
826,
828,
829,
830,
834,
835,
836,
1109,
1301,
1302,
5938,
5939,
5941,
5942,
5943,
5944,
5945,
5947,
5949,
5951,
5952,
5953,
5961,
5963,
5969,
5970,
5972,
5974,
5976,
5987,
5996,
5997,
5999,
6000,
6001,
6002,
6003,
6005,
6007,
7062,
7066,
7068,
7069,
7070,
7071,
7112,
7113,
7114,
7115,
7134,
7135,
7136,
7137,
7168,
7214,
7217,
7226,
7227,
7228,
7622,
7626,
7627,
7659,
7660,
7661,
7679,
7685,
7692,
7702,
7706,
7787,
7788,
7798,
7799,
7800,
7801,
7802,
7803,
7810,
7823,
7843,
7844,
7852,
7854,
7861,
7862,
7865,
7867,
7872,
7878,
7879,
7886,
7888,
7898,
7901,
7902,
7903,
7913,
7924,
7930,
7931,
7932,
7934,
7935,
7945,
7963,
7978,
7979,
7997,
8002,
8032,
8033,
8462,
8536,
8613,
8861,
8862,
8870,
8871,
8872,
8878,
8951,
9114,
9115,
10823,
10842,
10849,
10850,
10852,
};
void ves_icall_System_Array_InternalCreate (int,int,int,int,int);
int ves_icall_System_Array_GetCorElementTypeOfElementType_raw (int,int);
int ves_icall_System_Array_IsValueOfElementType_raw (int,int,int);
int ves_icall_System_Array_CanChangePrimitive (int,int,int);
int ves_icall_System_Array_FastCopy_raw (int,int,int,int,int,int);
int ves_icall_System_Array_GetLength_raw (int,int,int);
int ves_icall_System_Array_GetLowerBound_raw (int,int,int);
void ves_icall_System_Array_GetGenericValue_icall (int,int,int);
int ves_icall_System_Array_GetValueImpl_raw (int,int,int);
void ves_icall_System_Array_SetGenericValue_icall (int,int,int);
void ves_icall_System_Array_SetValueImpl_raw (int,int,int,int);
void ves_icall_System_Array_SetValueRelaxedImpl_raw (int,int,int,int);
void ves_icall_System_Runtime_RuntimeImports_Memmove (int,int,int);
void ves_icall_System_Buffer_BulkMoveWithWriteBarrier (int,int,int,int);
void ves_icall_System_Runtime_RuntimeImports_ZeroMemory (int,int);
int ves_icall_System_Delegate_AllocDelegateLike_internal_raw (int,int);
int ves_icall_System_Delegate_CreateDelegate_internal_raw (int,int,int,int,int);
int ves_icall_System_Delegate_GetVirtualMethod_internal_raw (int,int);
int ves_icall_System_Enum_GetEnumValuesAndNames_raw (int,int,int,int);
int ves_icall_System_Enum_ToObject_raw (int,int64_t,int);
int ves_icall_System_Enum_InternalGetCorElementType_raw (int,int);
int ves_icall_System_Enum_get_underlying_type_raw (int,int);
int ves_icall_System_Enum_InternalHasFlag_raw (int,int,int);
int ves_icall_System_Environment_get_ProcessorCount ();
int ves_icall_System_Environment_get_TickCount ();
int64_t ves_icall_System_Environment_get_TickCount64 ();
void ves_icall_System_Environment_FailFast_raw (int,int,int,int);
void ves_icall_System_GC_register_ephemeron_array_raw (int,int);
int ves_icall_System_GC_get_ephemeron_tombstone_raw (int);
void ves_icall_System_GC_SuppressFinalize_raw (int,int);
void ves_icall_System_GC_ReRegisterForFinalize_raw (int,int);
void ves_icall_System_GC_GetGCMemoryInfo (int,int,int,int,int,int);
int ves_icall_System_GC_AllocPinnedArray_raw (int,int,int);
int ves_icall_System_Object_MemberwiseClone_raw (int,int);
double ves_icall_System_Math_Abs_double (double);
float ves_icall_System_Math_Abs_single (float);
double ves_icall_System_Math_Ceiling (double);
double ves_icall_System_Math_Cos (double);
double ves_icall_System_Math_Floor (double);
double ves_icall_System_Math_Log10 (double);
double ves_icall_System_Math_Pow (double,double);
double ves_icall_System_Math_Sin (double);
double ves_icall_System_Math_Sqrt (double);
double ves_icall_System_Math_Tan (double);
double ves_icall_System_Math_ModF (double,int);
int ves_icall_RuntimeType_GetCorrespondingInflatedMethod_raw (int,int,int);
int ves_icall_RuntimeType_GetCorrespondingInflatedMethod_raw (int,int,int);
int ves_icall_RuntimeType_make_array_type_raw (int,int,int);
int ves_icall_RuntimeType_make_byref_type_raw (int,int);
int ves_icall_RuntimeType_MakePointerType_raw (int,int);
int ves_icall_RuntimeType_MakeGenericType_raw (int,int,int);
int ves_icall_RuntimeType_GetMethodsByName_native_raw (int,int,int,int,int);
int ves_icall_RuntimeType_GetPropertiesByName_native_raw (int,int,int,int,int);
int ves_icall_RuntimeType_GetConstructors_native_raw (int,int,int);
int ves_icall_System_Activator_CreateInstanceInternal_raw (int,int);
int ves_icall_RuntimeType_get_DeclaringMethod_raw (int,int);
int ves_icall_System_RuntimeType_getFullName_raw (int,int,int,int);
int ves_icall_RuntimeType_GetGenericArguments_raw (int,int,int);
int ves_icall_RuntimeType_GetGenericParameterPosition_raw (int,int);
int ves_icall_RuntimeType_GetEvents_native_raw (int,int,int,int);
int ves_icall_RuntimeType_GetFields_native_raw (int,int,int,int,int);
int ves_icall_RuntimeType_GetInterfaces_raw (int,int);
int ves_icall_RuntimeType_GetNestedTypes_native_raw (int,int,int,int,int);
int ves_icall_RuntimeType_get_DeclaringType_raw (int,int);
int ves_icall_RuntimeType_get_Name_raw (int,int);
int ves_icall_RuntimeType_get_Namespace_raw (int,int);
int ves_icall_RuntimeTypeHandle_GetAttributes_raw (int,int);
int ves_icall_reflection_get_token_raw (int,int);
int ves_icall_RuntimeTypeHandle_GetGenericTypeDefinition_impl_raw (int,int);
int ves_icall_RuntimeTypeHandle_GetCorElementType_raw (int,int);
int ves_icall_RuntimeTypeHandle_HasInstantiation_raw (int,int);
int ves_icall_RuntimeTypeHandle_IsComObject_raw (int,int);
int ves_icall_RuntimeTypeHandle_IsInstanceOfType_raw (int,int,int);
int ves_icall_RuntimeTypeHandle_HasReferences_raw (int,int);
int ves_icall_RuntimeTypeHandle_GetArrayRank_raw (int,int);
int ves_icall_RuntimeTypeHandle_GetAssembly_raw (int,int);
int ves_icall_RuntimeTypeHandle_GetElementType_raw (int,int);
int ves_icall_RuntimeTypeHandle_GetModule_raw (int,int);
int ves_icall_RuntimeTypeHandle_IsGenericVariable_raw (int,int);
int ves_icall_RuntimeTypeHandle_GetBaseType_raw (int,int);
int ves_icall_RuntimeTypeHandle_type_is_assignable_from_raw (int,int,int);
int ves_icall_RuntimeTypeHandle_IsGenericTypeDefinition_raw (int,int);
int ves_icall_RuntimeTypeHandle_GetGenericParameterInfo_raw (int,int);
int ves_icall_RuntimeTypeHandle_is_subclass_of (int,int);
int ves_icall_RuntimeTypeHandle_IsByRefLike_raw (int,int);
int ves_icall_System_RuntimeTypeHandle_internal_from_name_raw (int,int,int,int,int,int);
int ves_icall_System_String_FastAllocateString_raw (int,int);
int ves_icall_System_String_InternalIsInterned_raw (int,int);
int ves_icall_System_String_InternalIntern_raw (int,int);
int ves_icall_System_Type_internal_from_handle_raw (int,int);
int ves_icall_System_ValueType_InternalGetHashCode_raw (int,int,int);
int ves_icall_System_ValueType_Equals_raw (int,int,int,int);
int ves_icall_System_Threading_Interlocked_CompareExchange_Int (int,int,int);
void ves_icall_System_Threading_Interlocked_CompareExchange_Object (int,int,int,int);
int ves_icall_System_Threading_Interlocked_Decrement_Int (int);
int ves_icall_System_Threading_Interlocked_Increment_Int (int);
int64_t ves_icall_System_Threading_Interlocked_Increment_Long (int);
int ves_icall_System_Threading_Interlocked_Exchange_Int (int,int);
void ves_icall_System_Threading_Interlocked_Exchange_Object (int,int,int);
int64_t ves_icall_System_Threading_Interlocked_CompareExchange_Long (int,int64_t,int64_t);
int64_t ves_icall_System_Threading_Interlocked_Exchange_Long (int,int64_t);
int64_t ves_icall_System_Threading_Interlocked_Read_Long (int);
int ves_icall_System_Threading_Interlocked_Add_Int (int,int);
int64_t ves_icall_System_Threading_Interlocked_Add_Long (int,int64_t);
void ves_icall_System_Threading_Monitor_Monitor_Enter_raw (int,int);
void mono_monitor_exit_icall_raw (int,int);
int ves_icall_System_Threading_Monitor_Monitor_test_synchronised_raw (int,int);
void ves_icall_System_Threading_Monitor_Monitor_pulse_raw (int,int);
void ves_icall_System_Threading_Monitor_Monitor_pulse_all_raw (int,int);
int ves_icall_System_Threading_Monitor_Monitor_wait_raw (int,int,int,int);
void ves_icall_System_Threading_Monitor_Monitor_try_enter_with_atomic_var_raw (int,int,int,int,int);
int ves_icall_System_Threading_Thread_GetCurrentProcessorNumber_raw (int);
void ves_icall_System_Threading_Thread_InitInternal_raw (int,int);
int ves_icall_System_Threading_Thread_GetCurrentThread ();
void ves_icall_System_Threading_InternalThread_Thread_free_internal_raw (int,int);
int ves_icall_System_Threading_Thread_GetState_raw (int,int);
void ves_icall_System_Threading_Thread_SetState_raw (int,int,int);
void ves_icall_System_Threading_Thread_ClrState_raw (int,int,int);
void ves_icall_System_Threading_Thread_SetName_icall_raw (int,int,int,int);
int ves_icall_System_Threading_Thread_YieldInternal ();
void ves_icall_System_Threading_Thread_SetPriority_raw (int,int,int);
void ves_icall_System_Runtime_Loader_AssemblyLoadContext_PrepareForAssemblyLoadContextRelease_raw (int,int,int);
int ves_icall_System_Runtime_Loader_AssemblyLoadContext_GetLoadContextForAssembly_raw (int,int);
int ves_icall_System_Runtime_Loader_AssemblyLoadContext_InternalLoadFile_raw (int,int,int,int);
int ves_icall_System_Runtime_Loader_AssemblyLoadContext_InternalInitializeNativeALC_raw (int,int,int,int,int);
int ves_icall_System_Runtime_Loader_AssemblyLoadContext_InternalLoadFromStream_raw (int,int,int,int,int,int);
int ves_icall_System_Runtime_Loader_AssemblyLoadContext_InternalGetLoadedAssemblies_raw (int);
int ves_icall_System_GCHandle_InternalAlloc_raw (int,int,int);
void ves_icall_System_GCHandle_InternalFree_raw (int,int);
int ves_icall_System_GCHandle_InternalGet_raw (int,int);
void ves_icall_System_GCHandle_InternalSet_raw (int,int,int);
int ves_icall_System_Runtime_InteropServices_Marshal_GetLastPInvokeError ();
void ves_icall_System_Runtime_InteropServices_Marshal_SetLastPInvokeError (int);
void ves_icall_System_Runtime_InteropServices_Marshal_StructureToPtr_raw (int,int,int,int);
int ves_icall_System_Runtime_InteropServices_Marshal_IsPinnableType_raw (int,int);
int ves_icall_System_Runtime_InteropServices_NativeLibrary_LoadByName_raw (int,int,int,int,int,int);
int mono_object_hash_icall_raw (int,int);
int ves_icall_System_Runtime_CompilerServices_RuntimeHelpers_GetObjectValue_raw (int,int);
int ves_icall_System_Runtime_CompilerServices_RuntimeHelpers_GetUninitializedObjectInternal_raw (int,int);
void ves_icall_System_Runtime_CompilerServices_RuntimeHelpers_InitializeArray_raw (int,int,int);
int ves_icall_System_Runtime_CompilerServices_RuntimeHelpers_SufficientExecutionStack ();
int ves_icall_System_Reflection_Assembly_GetEntryAssembly_raw (int);
int ves_icall_System_Reflection_Assembly_InternalLoad_raw (int,int,int,int);
int ves_icall_System_Reflection_Assembly_InternalGetType_raw (int,int,int,int,int,int);
void mono_digest_get_public_token (int,int,int);
int ves_icall_System_Reflection_AssemblyName_GetNativeName (int);
int ves_icall_System_Reflection_AssemblyName_ParseAssemblyName (int,int,int,int);
int ves_icall_MonoCustomAttrs_GetCustomAttributesInternal_raw (int,int,int,int);
int ves_icall_MonoCustomAttrs_GetCustomAttributesDataInternal_raw (int,int);
int ves_icall_MonoCustomAttrs_IsDefinedInternal_raw (int,int,int);
int ves_icall_System_Reflection_FieldInfo_internal_from_handle_type_raw (int,int,int);
int ves_icall_System_Reflection_FieldInfo_get_marshal_info_raw (int,int);
int ves_icall_System_Reflection_RuntimeAssembly_GetManifestResourceNames_raw (int,int);
int ves_icall_System_Reflection_RuntimeAssembly_GetExportedTypes_raw (int,int);
int ves_icall_System_Reflection_RuntimeAssembly_get_location_raw (int,int);
int ves_icall_System_Reflection_RuntimeAssembly_get_code_base_raw (int,int);
int ves_icall_System_Reflection_RuntimeAssembly_get_fullname_raw (int,int);
int ves_icall_System_Reflection_RuntimeAssembly_GetManifestResourceInternal_raw (int,int,int,int,int);
int ves_icall_System_Reflection_Assembly_GetManifestModuleInternal_raw (int,int);
int ves_icall_System_Reflection_RuntimeAssembly_GetModulesInternal_raw (int,int);
void ves_icall_System_Reflection_RuntimeCustomAttributeData_ResolveArgumentsInternal_raw (int,int,int,int,int,int,int);
void ves_icall_RuntimeEventInfo_get_event_info_raw (int,int,int);
int ves_icall_reflection_get_token_raw (int,int);
int ves_icall_System_Reflection_EventInfo_internal_from_handle_type_raw (int,int,int);
int ves_icall_RuntimeFieldInfo_ResolveType_raw (int,int);
int ves_icall_RuntimeFieldInfo_GetParentType_raw (int,int,int);
int ves_icall_RuntimeFieldInfo_GetFieldOffset_raw (int,int);
int ves_icall_RuntimeFieldInfo_GetValueInternal_raw (int,int,int);
void ves_icall_RuntimeFieldInfo_SetValueInternal_raw (int,int,int,int);
int ves_icall_RuntimeFieldInfo_GetRawConstantValue_raw (int,int);
int ves_icall_reflection_get_token_raw (int,int);
void ves_icall_get_method_info_raw (int,int,int);
int ves_icall_get_method_attributes (int);
int ves_icall_System_Reflection_MonoMethodInfo_get_parameter_info_raw (int,int,int);
int ves_icall_System_MonoMethodInfo_get_retval_marshal_raw (int,int);
int ves_icall_System_Reflection_RuntimeMethodInfo_GetMethodFromHandleInternalType_native_raw (int,int,int,int);
int ves_icall_RuntimeMethodInfo_get_name_raw (int,int);
int ves_icall_RuntimeMethodInfo_get_base_method_raw (int,int,int);
int ves_icall_reflection_get_token_raw (int,int);
int ves_icall_InternalInvoke_raw (int,int,int,int,int);
void ves_icall_RuntimeMethodInfo_GetPInvoke_raw (int,int,int,int,int);
int ves_icall_RuntimeMethodInfo_MakeGenericMethod_impl_raw (int,int,int);
int ves_icall_RuntimeMethodInfo_GetGenericArguments_raw (int,int);
int ves_icall_RuntimeMethodInfo_GetGenericMethodDefinition_raw (int,int);
int ves_icall_RuntimeMethodInfo_get_IsGenericMethodDefinition_raw (int,int);
int ves_icall_RuntimeMethodInfo_get_IsGenericMethod_raw (int,int);
int ves_icall_InternalInvoke_raw (int,int,int,int,int);
int ves_icall_reflection_get_token_raw (int,int);
int ves_icall_System_Reflection_RuntimeModule_InternalGetTypes_raw (int,int);
int ves_icall_System_Reflection_RuntimeModule_ResolveMethodToken_raw (int,int,int,int,int,int);
int ves_icall_RuntimeParameterInfo_GetTypeModifiers_raw (int,int,int,int,int);
void ves_icall_RuntimePropertyInfo_get_property_info_raw (int,int,int,int);
int ves_icall_reflection_get_token_raw (int,int);
int ves_icall_System_Reflection_RuntimePropertyInfo_internal_from_handle_type_raw (int,int,int);
void ves_icall_AssemblyBuilder_basic_init_raw (int,int);
int ves_icall_CustomAttributeBuilder_GetBlob_raw (int,int,int,int,int,int,int,int);
void ves_icall_DynamicMethod_create_dynamic_method_raw (int,int);
void ves_icall_ModuleBuilder_basic_init_raw (int,int);
void ves_icall_ModuleBuilder_set_wrappers_type_raw (int,int,int);
int ves_icall_ModuleBuilder_getUSIndex_raw (int,int,int);
int ves_icall_ModuleBuilder_getToken_raw (int,int,int,int);
int ves_icall_ModuleBuilder_getMethodToken_raw (int,int,int,int);
void ves_icall_ModuleBuilder_RegisterToken_raw (int,int,int,int);
int ves_icall_TypeBuilder_create_runtime_class_raw (int,int);
int ves_icall_System_IO_Stream_HasOverriddenBeginEndRead_raw (int,int);
int ves_icall_System_IO_Stream_HasOverriddenBeginEndWrite_raw (int,int);
int ves_icall_Mono_RuntimeClassHandle_GetTypeFromClass (int);
void ves_icall_Mono_RuntimeGPtrArrayHandle_GPtrArrayFree (int);
void ves_icall_Mono_RuntimeMarshal_FreeAssemblyName (int,int);
int ves_icall_Mono_SafeStringMarshal_StringToUtf8 (int);
void ves_icall_Mono_SafeStringMarshal_GFree (int);
static void *corlib_icall_funcs [] = {
// token 187,
ves_icall_System_Array_InternalCreate,
// token 192,
ves_icall_System_Array_GetCorElementTypeOfElementType_raw,
// token 193,
ves_icall_System_Array_IsValueOfElementType_raw,
// token 194,
ves_icall_System_Array_CanChangePrimitive,
// token 195,
ves_icall_System_Array_FastCopy_raw,
// token 196,
ves_icall_System_Array_GetLength_raw,
// token 197,
ves_icall_System_Array_GetLowerBound_raw,
// token 198,
ves_icall_System_Array_GetGenericValue_icall,
// token 199,
ves_icall_System_Array_GetValueImpl_raw,
// token 200,
ves_icall_System_Array_SetGenericValue_icall,
// token 203,
ves_icall_System_Array_SetValueImpl_raw,
// token 204,
ves_icall_System_Array_SetValueRelaxedImpl_raw,
// token 321,
ves_icall_System_Runtime_RuntimeImports_Memmove,
// token 322,
ves_icall_System_Buffer_BulkMoveWithWriteBarrier,
// token 324,
ves_icall_System_Runtime_RuntimeImports_ZeroMemory,
// token 353,
ves_icall_System_Delegate_AllocDelegateLike_internal_raw,
// token 354,
ves_icall_System_Delegate_CreateDelegate_internal_raw,
// token 355,
ves_icall_System_Delegate_GetVirtualMethod_internal_raw,
// token 375,
ves_icall_System_Enum_GetEnumValuesAndNames_raw,
// token 376,
ves_icall_System_Enum_ToObject_raw,
// token 377,
ves_icall_System_Enum_InternalGetCorElementType_raw,
// token 378,
ves_icall_System_Enum_get_underlying_type_raw,
// token 379,
ves_icall_System_Enum_InternalHasFlag_raw,
// token 470,
ves_icall_System_Environment_get_ProcessorCount,
// token 471,
ves_icall_System_Environment_get_TickCount,
// token 472,
ves_icall_System_Environment_get_TickCount64,
// token 475,
ves_icall_System_Environment_FailFast_raw,
// token 514,
ves_icall_System_GC_register_ephemeron_array_raw,
// token 515,
ves_icall_System_GC_get_ephemeron_tombstone_raw,
// token 517,
ves_icall_System_GC_SuppressFinalize_raw,
// token 519,
ves_icall_System_GC_ReRegisterForFinalize_raw,
// token 521,
ves_icall_System_GC_GetGCMemoryInfo,
// token 523,
ves_icall_System_GC_AllocPinnedArray_raw,
// token 528,
ves_icall_System_Object_MemberwiseClone_raw,
// token 536,
ves_icall_System_Math_Abs_double,
// token 537,
ves_icall_System_Math_Abs_single,
// token 538,
ves_icall_System_Math_Ceiling,
// token 539,
ves_icall_System_Math_Cos,
// token 540,
ves_icall_System_Math_Floor,
// token 541,
ves_icall_System_Math_Log10,
// token 542,
ves_icall_System_Math_Pow,
// token 543,
ves_icall_System_Math_Sin,
// token 544,
ves_icall_System_Math_Sqrt,
// token 545,
ves_icall_System_Math_Tan,
// token 546,
ves_icall_System_Math_ModF,
// token 688,
ves_icall_RuntimeType_GetCorrespondingInflatedMethod_raw,
// token 689,
ves_icall_RuntimeType_GetCorrespondingInflatedMethod_raw,
// token 696,
ves_icall_RuntimeType_make_array_type_raw,
// token 699,
ves_icall_RuntimeType_make_byref_type_raw,
// token 701,
ves_icall_RuntimeType_MakePointerType_raw,
// token 706,
ves_icall_RuntimeType_MakeGenericType_raw,
// token 707,
ves_icall_RuntimeType_GetMethodsByName_native_raw,
// token 709,
ves_icall_RuntimeType_GetPropertiesByName_native_raw,
// token 710,
ves_icall_RuntimeType_GetConstructors_native_raw,
// token 714,
ves_icall_System_Activator_CreateInstanceInternal_raw,
// token 715,
ves_icall_RuntimeType_get_DeclaringMethod_raw,
// token 716,
ves_icall_System_RuntimeType_getFullName_raw,
// token 717,
ves_icall_RuntimeType_GetGenericArguments_raw,
// token 719,
ves_icall_RuntimeType_GetGenericParameterPosition_raw,
// token 720,
ves_icall_RuntimeType_GetEvents_native_raw,
// token 721,
ves_icall_RuntimeType_GetFields_native_raw,
// token 724,
ves_icall_RuntimeType_GetInterfaces_raw,
// token 725,
ves_icall_RuntimeType_GetNestedTypes_native_raw,
// token 728,
ves_icall_RuntimeType_get_DeclaringType_raw,
// token 729,
ves_icall_RuntimeType_get_Name_raw,
// token 730,
ves_icall_RuntimeType_get_Namespace_raw,
// token 798,
ves_icall_RuntimeTypeHandle_GetAttributes_raw,
// token 799,
ves_icall_reflection_get_token_raw,
// token 801,
ves_icall_RuntimeTypeHandle_GetGenericTypeDefinition_impl_raw,
// token 809,
ves_icall_RuntimeTypeHandle_GetCorElementType_raw,
// token 810,
ves_icall_RuntimeTypeHandle_HasInstantiation_raw,
// token 811,
ves_icall_RuntimeTypeHandle_IsComObject_raw,
// token 812,
ves_icall_RuntimeTypeHandle_IsInstanceOfType_raw,
// token 813,
ves_icall_RuntimeTypeHandle_HasReferences_raw,
// token 817,
ves_icall_RuntimeTypeHandle_GetArrayRank_raw,
// token 818,
ves_icall_RuntimeTypeHandle_GetAssembly_raw,
// token 819,
ves_icall_RuntimeTypeHandle_GetElementType_raw,
// token 820,
ves_icall_RuntimeTypeHandle_GetModule_raw,
// token 821,
ves_icall_RuntimeTypeHandle_IsGenericVariable_raw,
// token 822,
ves_icall_RuntimeTypeHandle_GetBaseType_raw,
// token 824,
ves_icall_RuntimeTypeHandle_type_is_assignable_from_raw,
// token 825,
ves_icall_RuntimeTypeHandle_IsGenericTypeDefinition_raw,
// token 826,
ves_icall_RuntimeTypeHandle_GetGenericParameterInfo_raw,
// token 828,
ves_icall_RuntimeTypeHandle_is_subclass_of,
// token 829,
ves_icall_RuntimeTypeHandle_IsByRefLike_raw,
// token 830,
ves_icall_System_RuntimeTypeHandle_internal_from_name_raw,
// token 834,
ves_icall_System_String_FastAllocateString_raw,
// token 835,
ves_icall_System_String_InternalIsInterned_raw,
// token 836,
ves_icall_System_String_InternalIntern_raw,
// token 1109,
ves_icall_System_Type_internal_from_handle_raw,
// token 1301,
ves_icall_System_ValueType_InternalGetHashCode_raw,
// token 1302,
ves_icall_System_ValueType_Equals_raw,
// token 5938,
ves_icall_System_Threading_Interlocked_CompareExchange_Int,
// token 5939,
ves_icall_System_Threading_Interlocked_CompareExchange_Object,
// token 5941,
ves_icall_System_Threading_Interlocked_Decrement_Int,
// token 5942,
ves_icall_System_Threading_Interlocked_Increment_Int,
// token 5943,
ves_icall_System_Threading_Interlocked_Increment_Long,
// token 5944,
ves_icall_System_Threading_Interlocked_Exchange_Int,
// token 5945,
ves_icall_System_Threading_Interlocked_Exchange_Object,
// token 5947,
ves_icall_System_Threading_Interlocked_CompareExchange_Long,
// token 5949,
ves_icall_System_Threading_Interlocked_Exchange_Long,
// token 5951,
ves_icall_System_Threading_Interlocked_Read_Long,
// token 5952,
ves_icall_System_Threading_Interlocked_Add_Int,
// token 5953,
ves_icall_System_Threading_Interlocked_Add_Long,
// token 5961,
ves_icall_System_Threading_Monitor_Monitor_Enter_raw,
// token 5963,
mono_monitor_exit_icall_raw,
// token 5969,
ves_icall_System_Threading_Monitor_Monitor_test_synchronised_raw,
// token 5970,
ves_icall_System_Threading_Monitor_Monitor_pulse_raw,
// token 5972,
ves_icall_System_Threading_Monitor_Monitor_pulse_all_raw,
// token 5974,
ves_icall_System_Threading_Monitor_Monitor_wait_raw,
// token 5976,
ves_icall_System_Threading_Monitor_Monitor_try_enter_with_atomic_var_raw,
// token 5987,
ves_icall_System_Threading_Thread_GetCurrentProcessorNumber_raw,
// token 5996,
ves_icall_System_Threading_Thread_InitInternal_raw,
// token 5997,
ves_icall_System_Threading_Thread_GetCurrentThread,
// token 5999,
ves_icall_System_Threading_InternalThread_Thread_free_internal_raw,
// token 6000,
ves_icall_System_Threading_Thread_GetState_raw,
// token 6001,
ves_icall_System_Threading_Thread_SetState_raw,
// token 6002,
ves_icall_System_Threading_Thread_ClrState_raw,
// token 6003,
ves_icall_System_Threading_Thread_SetName_icall_raw,
// token 6005,
ves_icall_System_Threading_Thread_YieldInternal,
// token 6007,
ves_icall_System_Threading_Thread_SetPriority_raw,
// token 7062,
ves_icall_System_Runtime_Loader_AssemblyLoadContext_PrepareForAssemblyLoadContextRelease_raw,
// token 7066,
ves_icall_System_Runtime_Loader_AssemblyLoadContext_GetLoadContextForAssembly_raw,
// token 7068,
ves_icall_System_Runtime_Loader_AssemblyLoadContext_InternalLoadFile_raw,
// token 7069,
ves_icall_System_Runtime_Loader_AssemblyLoadContext_InternalInitializeNativeALC_raw,
// token 7070,
ves_icall_System_Runtime_Loader_AssemblyLoadContext_InternalLoadFromStream_raw,
// token 7071,
ves_icall_System_Runtime_Loader_AssemblyLoadContext_InternalGetLoadedAssemblies_raw,
// token 7112,
ves_icall_System_GCHandle_InternalAlloc_raw,
// token 7113,
ves_icall_System_GCHandle_InternalFree_raw,
// token 7114,
ves_icall_System_GCHandle_InternalGet_raw,
// token 7115,
ves_icall_System_GCHandle_InternalSet_raw,
// token 7134,
ves_icall_System_Runtime_InteropServices_Marshal_GetLastPInvokeError,
// token 7135,
ves_icall_System_Runtime_InteropServices_Marshal_SetLastPInvokeError,
// token 7136,
ves_icall_System_Runtime_InteropServices_Marshal_StructureToPtr_raw,
// token 7137,
ves_icall_System_Runtime_InteropServices_Marshal_IsPinnableType_raw,
// token 7168,
ves_icall_System_Runtime_InteropServices_NativeLibrary_LoadByName_raw,
// token 7214,
mono_object_hash_icall_raw,
// token 7217,
ves_icall_System_Runtime_CompilerServices_RuntimeHelpers_GetObjectValue_raw,
// token 7226,
ves_icall_System_Runtime_CompilerServices_RuntimeHelpers_GetUninitializedObjectInternal_raw,
// token 7227,
ves_icall_System_Runtime_CompilerServices_RuntimeHelpers_InitializeArray_raw,
// token 7228,
ves_icall_System_Runtime_CompilerServices_RuntimeHelpers_SufficientExecutionStack,
// token 7622,
ves_icall_System_Reflection_Assembly_GetEntryAssembly_raw,
// token 7626,
ves_icall_System_Reflection_Assembly_InternalLoad_raw,
// token 7627,
ves_icall_System_Reflection_Assembly_InternalGetType_raw,
// token 7659,
mono_digest_get_public_token,
// token 7660,
ves_icall_System_Reflection_AssemblyName_GetNativeName,
// token 7661,
ves_icall_System_Reflection_AssemblyName_ParseAssemblyName,
// token 7679,
ves_icall_MonoCustomAttrs_GetCustomAttributesInternal_raw,
// token 7685,
ves_icall_MonoCustomAttrs_GetCustomAttributesDataInternal_raw,
// token 7692,
ves_icall_MonoCustomAttrs_IsDefinedInternal_raw,
// token 7702,
ves_icall_System_Reflection_FieldInfo_internal_from_handle_type_raw,
// token 7706,
ves_icall_System_Reflection_FieldInfo_get_marshal_info_raw,
// token 7787,
ves_icall_System_Reflection_RuntimeAssembly_GetManifestResourceNames_raw,
// token 7788,
ves_icall_System_Reflection_RuntimeAssembly_GetExportedTypes_raw,
// token 7798,
ves_icall_System_Reflection_RuntimeAssembly_get_location_raw,
// token 7799,
ves_icall_System_Reflection_RuntimeAssembly_get_code_base_raw,
// token 7800,
ves_icall_System_Reflection_RuntimeAssembly_get_fullname_raw,
// token 7801,
ves_icall_System_Reflection_RuntimeAssembly_GetManifestResourceInternal_raw,
// token 7802,
ves_icall_System_Reflection_Assembly_GetManifestModuleInternal_raw,
// token 7803,
ves_icall_System_Reflection_RuntimeAssembly_GetModulesInternal_raw,
// token 7810,
ves_icall_System_Reflection_RuntimeCustomAttributeData_ResolveArgumentsInternal_raw,
// token 7823,
ves_icall_RuntimeEventInfo_get_event_info_raw,
// token 7843,
ves_icall_reflection_get_token_raw,
// token 7844,
ves_icall_System_Reflection_EventInfo_internal_from_handle_type_raw,
// token 7852,
ves_icall_RuntimeFieldInfo_ResolveType_raw,
// token 7854,
ves_icall_RuntimeFieldInfo_GetParentType_raw,
// token 7861,
ves_icall_RuntimeFieldInfo_GetFieldOffset_raw,
// token 7862,
ves_icall_RuntimeFieldInfo_GetValueInternal_raw,
// token 7865,
ves_icall_RuntimeFieldInfo_SetValueInternal_raw,
// token 7867,
ves_icall_RuntimeFieldInfo_GetRawConstantValue_raw,
// token 7872,
ves_icall_reflection_get_token_raw,
// token 7878,
ves_icall_get_method_info_raw,
// token 7879,
ves_icall_get_method_attributes,
// token 7886,
ves_icall_System_Reflection_MonoMethodInfo_get_parameter_info_raw,
// token 7888,
ves_icall_System_MonoMethodInfo_get_retval_marshal_raw,
// token 7898,
ves_icall_System_Reflection_RuntimeMethodInfo_GetMethodFromHandleInternalType_native_raw,
// token 7901,
ves_icall_RuntimeMethodInfo_get_name_raw,
// token 7902,
ves_icall_RuntimeMethodInfo_get_base_method_raw,
// token 7903,
ves_icall_reflection_get_token_raw,
// token 7913,
ves_icall_InternalInvoke_raw,
// token 7924,
ves_icall_RuntimeMethodInfo_GetPInvoke_raw,
// token 7930,
ves_icall_RuntimeMethodInfo_MakeGenericMethod_impl_raw,
// token 7931,
ves_icall_RuntimeMethodInfo_GetGenericArguments_raw,
// token 7932,
ves_icall_RuntimeMethodInfo_GetGenericMethodDefinition_raw,
// token 7934,
ves_icall_RuntimeMethodInfo_get_IsGenericMethodDefinition_raw,
// token 7935,
ves_icall_RuntimeMethodInfo_get_IsGenericMethod_raw,
// token 7945,
ves_icall_InternalInvoke_raw,
// token 7963,
ves_icall_reflection_get_token_raw,
// token 7978,
ves_icall_System_Reflection_RuntimeModule_InternalGetTypes_raw,
// token 7979,
ves_icall_System_Reflection_RuntimeModule_ResolveMethodToken_raw,
// token 7997,
ves_icall_RuntimeParameterInfo_GetTypeModifiers_raw,
// token 8002,
ves_icall_RuntimePropertyInfo_get_property_info_raw,
// token 8032,
ves_icall_reflection_get_token_raw,
// token 8033,
ves_icall_System_Reflection_RuntimePropertyInfo_internal_from_handle_type_raw,
// token 8462,
ves_icall_AssemblyBuilder_basic_init_raw,
// token 8536,
ves_icall_CustomAttributeBuilder_GetBlob_raw,
// token 8613,
ves_icall_DynamicMethod_create_dynamic_method_raw,
// token 8861,
ves_icall_ModuleBuilder_basic_init_raw,
// token 8862,
ves_icall_ModuleBuilder_set_wrappers_type_raw,
// token 8870,
ves_icall_ModuleBuilder_getUSIndex_raw,
// token 8871,
ves_icall_ModuleBuilder_getToken_raw,
// token 8872,
ves_icall_ModuleBuilder_getMethodToken_raw,
// token 8878,
ves_icall_ModuleBuilder_RegisterToken_raw,
// token 8951,
ves_icall_TypeBuilder_create_runtime_class_raw,
// token 9114,
ves_icall_System_IO_Stream_HasOverriddenBeginEndRead_raw,
// token 9115,
ves_icall_System_IO_Stream_HasOverriddenBeginEndWrite_raw,
// token 10823,
ves_icall_Mono_RuntimeClassHandle_GetTypeFromClass,
// token 10842,
ves_icall_Mono_RuntimeGPtrArrayHandle_GPtrArrayFree,
// token 10849,
ves_icall_Mono_RuntimeMarshal_FreeAssemblyName,
// token 10850,
ves_icall_Mono_SafeStringMarshal_StringToUtf8,
// token 10852,
ves_icall_Mono_SafeStringMarshal_GFree,
};
static uint8_t corlib_icall_handles [] = {
0,
1,
1,
0,
1,
1,
1,
0,
1,
0,
1,
1,
0,
0,
0,
1,
1,
1,
1,
1,
1,
1,
1,
0,
0,
0,
1,
1,
1,
1,
1,
0,
1,
1,
0,
0,
0,
0,
0,
0,
0,
0,
0,
0,
0,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
0,
1,
1,
1,
1,
1,
1,
1,
1,
0,
0,
0,
0,
0,
0,
0,
0,
0,
0,
0,
0,
1,
1,
1,
1,
1,
1,
1,
1,
1,
0,
1,
1,
1,
1,
1,
0,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
0,
0,
1,
1,
1,
1,
1,
1,
1,
0,
1,
1,
1,
0,
0,
0,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
0,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
0,
0,
0,
0,
0,
};

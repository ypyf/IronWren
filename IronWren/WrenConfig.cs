﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

#if true
#endif

namespace IronWren
{
    [StructLayout(LayoutKind.Sequential)]
    public class WrenConfig
    {
        internal Config config;

        /// <summary>
        /// The callback Wren will use to allocate, reallocate, and deallocate memory.
        ///
        /// If `NULL`, defaults to a built-in function that uses `realloc` and `free`.
        /// </summary>
        public WrenReallocate Reallocate
        {
            get { return config.reallocate; }
            set { config.reallocate = value; }
        }

        /// <summary>
        /// The callback Wren uses to load a module.
        ///
        /// Since Wren does not talk directly to the file system, it relies on the
        /// embedder to phyisically locate and read the source code for a module. The
        /// first time an import appears, Wren will call this and pass in the name of
        /// the module being imported. The VM should return the soure code for that and
        /// Wren will take ownership over it.
        ///
        /// This will only be called once for any given module name. Wren caches the
        /// result internally so subsequent imports of the same module will use the
        /// previous source and not call this.
        ///
        /// If a module with the given name could not be found by the embedder, it
        /// should return NULL and Wren will report that as a runtime error.
        /// </summary>
        public WrenLoadModule LoadModule
        {
            get { return config.loadModule; }
            set { config.loadModule = value; }
        }

        /// <summary>
        /// The callback Wren uses to find a foreign method and bind it to a class.
        ///
        /// When a foreign method is declared in a class, this will be called with the
        /// foreign method's module, class, and signature when the class body is
        /// executed. It should return a pointer to the foreign function that will be
        /// bound to that method.
        ///
        /// If the foreign function could not be found, this should return NULL and
        /// Wren will report it as runtime error.
        /// </summary>
        public WrenBindForeignMethod bindForeignMethod;

        /// <summary>
        /// The callback Wren uses to find a foreign class and get its foreign methods.
        ///
        /// When a foreign class is declared, this will be called with the class's
        /// module and name when the class body is executed. It should return the
        /// foreign functions uses to allocate and (optionally) finalize the bytes
        /// stored in the foreign object when an instance is created.
        /// </summary>
        public WrenBindForeignClass bindForeignClass;

        /// <summary>
        /// The callback Wren uses to display text when `System.print()` or the other
        /// related functions are called.
        ///
        /// If this is `NULL`, Wren discards any printed text.
        /// </summary>
        public WrenWrite write
        {
            get { return config.write; }
            set { config.write = value; }
        }

        /// <summary>
        /// The callback Wren uses to report errors.
        ///
        /// When an error occurs, this will be called with the module name, line
        /// number, and an error message. If this is `NULL`, Wren doesn't report any
        /// errors.
        /// </summary>
        public WrenError error;

        /// <summary>
        /// The number of bytes Wren will allocate before triggering the first garbage
        /// collection.
        ///
        /// If zero, defaults to 10MB.
        /// </summary>
        public int initialHeapSize
        {
            get { return config.initialHeapSize; }
            set { config.initialHeapSize = value; }
        }

        /// <summary>
        /// After a collection occurs, the threshold for the next collection is
        /// determined based on the number of bytes remaining in use. This allows Wren
        /// to shrink its memory usage automatically after reclaiming a large amount
        /// of memory.
        ///
        /// This can be used to ensure that the heap does not get too small, which can
        /// in turn lead to a large number of collections afterwards as the heap grows
        /// back to a usable size.
        ///
        /// If zero, defaults to 1MB.
        /// </summary>
        public int minHeapSize;

        /// <summary>
        /// Wren will grow (and shrink) the heap automatically as the number of bytes
        /// remaining in use after a collection changes. This number determines the
        /// amount of additional memory Wren will use after a collection, as a
        /// percentage of the current heap size.
        ///
        /// For example, say that this is 50. After a garbage collection, Wren there
        /// are 400 bytes of memory still in use. That means the next collection will
        /// be triggered after a total of 600 bytes are allocated (including the 400
        /// already in use.
        ///
        /// Setting this to a smaller number wastes less memory, but triggers more
        /// frequent garbage collections.
        ///
        /// If zero, defaults to 50.
        /// </summary>
        public int heapGrowthPercent;

        internal struct Config
        {
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public WrenReallocate reallocate;

            [MarshalAs(UnmanagedType.FunctionPtr)]
            public WrenLoadModule loadModule;

            [MarshalAs(UnmanagedType.FunctionPtr)]
            public WrenBindForeignMethod bindForeignMethod;

            [MarshalAs(UnmanagedType.FunctionPtr)]
            public WrenBindForeignClass bindForeignClass;

            [MarshalAs(UnmanagedType.FunctionPtr)]
            public WrenWrite write;

            [MarshalAs(UnmanagedType.FunctionPtr)]
            public WrenError error;

            public int initialHeapSize;

            public int minHeapSize;

            public int heapGrowthPercent;
        }

        /// <summary>
        ///
        /// </summary>
        public WrenConfig()
        {
            initConfiguration(out config);
        }

        [DllImport("wren_static_d.dll", EntryPoint = "wrenInitConfiguration", CallingConvention = CallingConvention.Cdecl)]
        private static extern void initConfiguration(out Config config);
    }
}
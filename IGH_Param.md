#region сборка Grasshopper, Version=6.35.21222.17000, Culture=neutral, PublicKeyToken=dda4f5ec2cd80803
// D:\Grasshopper Projects\repos\3D Braid\packages\Grasshopper.6.35.21222.17001\lib\net45\Grasshopper.dll
// Decompiled with ICSharpCode.Decompiler 8.1.1.7464
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using Grasshopper.Kernel.Data;

namespace Grasshopper.Kernel;

//
// Сводка:
//     Base interface for all Parameter types in Grasshopper. Do not implement this
//     interface from scratch, derive from GH_Param or GH_PersistentParam instead.
public interface IGH_Param : IGH_ActiveObject
{
    //
    // Сводка:
    //     Gets the parameter kind. The kind is evaluated lazily and cached.
    GH_ParamKind Kind { get; }

    //
    // Сводка:
    //     Gets the type of the current data state.
    GH_ParamData DataType { get; }

    //
    // Сводка:
    //     Gets the Framework Type descriptor for the stored Data.
    Type Type { get; }

    //
    // Сводка:
    //     Gets a human readable description of the data stored in this parameter.
    string TypeName { get; }

    //
    // Сводка:
    //     Gets all the StateTags that are associated with this parameter. A state tag is
    //     a visual feedback icon that represents specific internal settings.
    GH_StateTagList StateTags { get; }

    //
    // Сводка:
    //     Gets or sets the wire display style for this parameter. Wire display only affects
    //     the wires connected to the parameter input.
    GH_ParamWireDisplay WireDisplay { get; set; }

    //
    // Сводка:
    //     Gets or sets whether or not this parameter is considered optional by the owner
    //     component. Empty, non-optional parameters prevent the component from being solved.
    bool Optional { get; set; }

    //
    // Сводка:
    //     Gets or sets the data mapping of this Parameter.
    GH_DataMapping DataMapping { get; set; }

    //
    // Сводка:
    //     Gets or sets the Access level for this parameter.
    GH_ParamAccess Access { get; set; }

    //
    // Сводка:
    //     Gets or sets the data reverse modifier of this parameter.
    bool Reverse { get; set; }

    //
    // Сводка:
    //     Gets or sets the simplify modifier for this parameter.
    bool Simplify { get; set; }

    //
    // Сводка:
    //     Gets a list of source parameters. Do not modify this list, if you wish to add
    //     or remove sources, use dedicated functions like AddSource() and RemoveSource
    //     instead.
    //
    // Возврат:
    //     The sources for this parameter.
    IList<IGH_Param> Sources { get; }

    //
    // Сводка:
    //     Gets the number of sources for this parameter.
    int SourceCount { get; }

    //
    // Сводка:
    //     Gets a value indicating whether or not this parameter maintains proxy sources.
    //     Proxy sources are used during file IO, when actual sources might not be available
    //     yet. Once an IO operation has been completed there should be no more proxy sources.
    //
    //
    // Возврат:
    //     True if the parameter has at least one proxy source, false if not.
    bool HasProxySources { get; }

    //
    // Сводка:
    //     Gets the number of proxy sources for this parameter. Proxy sources are used during
    //     file IO, when actual sources might not be available yet. Once an IO operation
    //     has been completed there should be no more proxy sources.
    //
    // Возврат:
    //     The number of proxy sources associated with this parameter.
    int ProxySourceCount { get; }

    //
    // Сводка:
    //     Gets a list of all the recipients of this parameter. I.e. a recipient has this
    //     parameter as one of the sources. The Recipient list is maintained by the parameter,
    //     do not modify it yourself.
    IList<IGH_Param> Recipients { get; }

    //
    // Сводка:
    //     Gets the total number of volatile data items stored in this parameter.
    int VolatileDataCount { get; }

    //
    // Сводка:
    //     Gets the instance of the volatile data tree stored in this parameter.
    IGH_Structure VolatileData { get; }

    //
    // Сводка:
    //     Remove all post-process effects.
    void RemoveEffects();

    //
    // Сводка:
    //     Append a new Source parameter to the end of the Sources list. Sources provide
    //     this parameter with data at runtime.
    //
    // Параметры:
    //   source:
    //     Source to append.
    void AddSource(IGH_Param source);

    //
    // Сводка:
    //     Insert a new Source parameter into the Sources list. Sources provide this parameter
    //     with data at runtime.
    //
    // Параметры:
    //   source:
    //     Source to append.
    //
    //   index:
    //     Insertion index of source.
    void AddSource(IGH_Param source, int index);

    //
    // Сводка:
    //     Remove the specified source from this parameter.
    //
    // Параметры:
    //   source:
    //     Source to remove.
    void RemoveSource(IGH_Param source);

    //
    // Сводка:
    //     Remove the specified source from this parameter.
    //
    // Параметры:
    //   source_id:
    //     InstanceID of source to remove.
    void RemoveSource(Guid source_id);

    //
    // Сводка:
    //     Remove all sources from this parameter.
    void RemoveAllSources();

    //
    // Сводка:
    //     Replace an existing source with a new one. If the old_source does not exist in
    //     this parameter, nothing happens.
    //
    // Параметры:
    //   old_source:
    //     Source to replace.
    //
    //   new_source:
    //     Source to replace with.
    void ReplaceSource(IGH_Param old_source, IGH_Param new_source);

    //
    // Сводка:
    //     Replace an existing source with a new one. If the old_source does not exist in
    //     this parameter, nothing happens.
    //
    // Параметры:
    //   old_source_id:
    //     Source to replace.
    //
    //   new_source:
    //     Source to replace with.
    void ReplaceSource(Guid old_source_id, IGH_Param new_source);

    //
    // Сводка:
    //     Attempt to replace all proxy sources with real sources. Proxy sources are used
    //     during file IO, when actual sources might not be available yet. Once an IO operation
    //     has been completed there should be no more proxy sources.
    //
    // Параметры:
    //   Document:
    //     The document from which to harvest the real source parameters.
    //
    // Возврат:
    //     True on success, false on failure.
    bool RelinkProxySources(GH_Document document);

    //
    // Сводка:
    //     Remove all proxy sources without attempting to relink them.
    void ClearProxySources();

    //
    // Сводка:
    //     Convert all proper source parameters into proxy sources.
    void CreateProxySources();

    //
    // Сводка:
    //     Inserts an item of volatile data into the data structure.
    //
    // Параметры:
    //   path:
    //     The branch path of the data. If the path doesn't exist yet, it will be created.
    //
    //
    //   index:
    //     The item index of the data. If the path doesn't contain the index yet, it will
    //     be enlarged to encompass the index.
    //
    //   data:
    //     The data to store.
    //
    // Возврат:
    //     True on success, False on failure. If the data cannot be converted, the topology
    //     will remain unmolested.
    bool AddVolatileData(GH_Path path, int index, object data);

    //
    // Сводка:
    //     Inserts a list of items into the data structure.
    //
    // Параметры:
    //   path:
    //     The branch path of the data. If the path doesn't exist yet, it will be created.
    //
    //
    //   list:
    //     The data list to store.
    bool AddVolatileDataList(GH_Path path, IEnumerable list);

    //
    // Сводка:
    //     Insert an entire data tree into this parameter.
    //
    // Параметры:
    //   tree:
    //     Data tree to insert.
    //
    // Возврат:
    //     True on success, false on failure.
    bool AddVolatileDataTree(IGH_Structure tree);

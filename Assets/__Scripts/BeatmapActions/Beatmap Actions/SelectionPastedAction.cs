﻿using System.Collections.Generic;
using LiteNetLib.Utils;

public class SelectionPastedAction : BeatmapAction
{
    private IEnumerable<BeatmapObject> removed;

    public SelectionPastedAction() : base() { }

    public SelectionPastedAction(IEnumerable<BeatmapObject> pasteData, IEnumerable<BeatmapObject> removed) :
        base(pasteData) => this.removed = removed;

    public override void Undo(BeatmapActionContainer.BeatmapActionParams param)
    {
        foreach (var obj in Data)
            DeleteObject(obj, false);
        foreach (var obj in removed)
            SpawnObject(obj);
        RefreshPools(removed);
    }

    public override void Redo(BeatmapActionContainer.BeatmapActionParams param)
    {
        SelectionController.DeselectAll();
        foreach (var obj in Data)
        {
            SpawnObject(obj);

            if (!Networked)
            {
                SelectionController.Select(obj, true, false, false);
            }
        }

        foreach (var obj in removed)
            DeleteObject(obj, false);
        RefreshPools(Data);
    }

    public override void Serialize(NetDataWriter writer)
    {
        SerializeBeatmapObjectList(writer, Data);
        SerializeBeatmapObjectList(writer, removed);
    }

    public override void Deserialize(NetDataReader reader)
    {
        Data = DeserializeBeatmapObjectList(reader);
        removed = DeserializeBeatmapObjectList(reader);
    }
}

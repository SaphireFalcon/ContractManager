using Brutal.ImGuiApi;
using ContractManager.ContractBlueprint;
using ContractManager.Mission;
using KSA;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContractManager.GUI
{
    internal enum PopupType {
        Popup,
        Modal
    }

    internal class PopupWindow
    {
        // Title of popup window, used when contract is null
        internal string title { get; set; } = string.Empty;
        
        // UID of popup window, used when action is null
        internal string uid {  get; set; } = string.Empty;

        // Message to show in popup window, used when action is null
        internal string messageToShow {  get; set; } = string.Empty;

        // type of popup to show.
        internal PopupType popupType { get; set; } = PopupType.Popup;

        // Flag to draw the popup or not.
        internal bool drawPopup { get; set; } = true;

        internal PopupWindow() { }

        internal virtual void DrawPopup()
        {
            bool isDrawing = false;
            ImGui.SetNextWindowSizeConstraints(
                new Brutal.Numerics.float2 { X = 300.0f, Y = 200.0f },
                new Brutal.Numerics.float2 { X = float.PositiveInfinity, Y = float.PositiveInfinity }  // no max size
            );
            if (this.popupType == PopupType.Popup)
            {
                // Using a normal window is nicer instead of a popup.
                isDrawing = ImGui.Begin(this.title);
            }
            else
            if (this.popupType == PopupType.Modal)
            {
                // If there is a blocking modal pop, best to put the game speed to 0.
                KSA.Universe.SetSimulationSpeed(0);
                ImGui.OpenPopup(this.title);
                isDrawing = ImGui.BeginPopupModal(this.title);
            }
            if(isDrawing)
            {
                ImGui.TextWrapped(this.messageToShow);
                if (ImGui.Button("OK"))
                {
                    if (this.popupType == PopupType.Modal)
                    {
                        ImGui.CloseCurrentPopup();
                    }
                    this.drawPopup = false;
                }
                
                if (this.popupType == PopupType.Popup)
                {
                    ImGui.End();
                }
                if (this.popupType == PopupType.Modal)
                {
                    ImGui.EndPopup();
                }
            }
        }
    }
    
    internal class ExportModal : PopupWindow
    {
        internal MissionBlueprint? missionBlueprint = null;
        internal ContractBlueprint.ContractBlueprint? contractBlueprint = null;
        internal int modDirectoryIndex = -1;
        
        internal ExportModal() { }

        internal override void DrawPopup()
        {
            // If the blueprint is to be saved to a file, create a popup to ask where or to confirm to save it.
            // Find mods in the documents game mods folder and offer to save in one of them.
            ImGui.OpenPopup("Where to export?");
            if (ImGui.BeginPopupModal("Where to export?"))
            {
                ImGui.Text(messageToShow);
                ImGui.Separator();

                bool isLoadedFromFile = (  
                    (this.missionBlueprint != null && !String.IsNullOrEmpty(this.missionBlueprint.loadedFromFilePath)) ||
                    ( this.contractBlueprint != null && !String.IsNullOrEmpty(this.contractBlueprint.loadedFromFilePath))
                );
                string[] modeDirectories = Directory.GetDirectories(ModLibrary.LocalModsFolderPath);
                if (!isLoadedFromFile)
                {
                    ImGuiTreeNodeFlags modsTreeNodeFlags = ImGuiTreeNodeFlags.DrawLinesToNodes | ImGuiTreeNodeFlags.DefaultOpen;
                    if (ImGui.TreeNodeEx("mods:", modsTreeNodeFlags))
                    {
                            foreach (var (modDirectory, index) in modeDirectories.Select((v, i) => (v, i)))
                        {
                            ImGuiTreeNodeFlags modNodeTreeNodeFlags = ImGuiTreeNodeFlags.Leaf | ImGuiTreeNodeFlags.NoTreePushOnOpen | ImGuiTreeNodeFlags.SpanLabelWidth;
                            if (index == modDirectoryIndex)
                            {
                                modNodeTreeNodeFlags |= ImGuiTreeNodeFlags.Selected;
                            }
                            if (ImGui.TreeNodeEx(String.Format("{0}##{1}", Path.GetFileName(modDirectory), index), modNodeTreeNodeFlags))
                            {
                                if (ImGui.IsItemClicked())
                                {
                                    modDirectoryIndex = index;
                                    Console.WriteLine($"[CM] clicked {Path.GetFileName(modDirectory)} at {modDirectoryIndex} -> {modeDirectories[modDirectoryIndex]}");
                                }
                                //ImGui.TreePop();
                            }
                        }
                        ImGui.TreePop();
                    }
                }

                if (ImGui.Button("Cancel", new Brutal.Numerics.float2 {X = 120.0f, Y = 0.0f })) {
                    this.drawPopup = false;
                    ImGui.CloseCurrentPopup();
                }
                ImGui.SetItemDefaultFocus();
                ImGui.SameLine();
                if (ImGui.Button("Export", new Brutal.Numerics.float2 {X = 120.0f, Y = 0.0f }))
                {
                    string exportFilePath = string.Empty;
                    if (!isLoadedFromFile)
                    {
                        if (modDirectoryIndex >= 0 && modDirectoryIndex < modeDirectories.Length)
                        {
                            string modDirectory = modeDirectories[modDirectoryIndex];
                            if (this.contractBlueprint != null) {
                                exportFilePath = Path.Combine(modDirectory, @"contracts\", this.contractBlueprint.uid + ".xml");
                                this.contractBlueprint.loadedFromFilePath = exportFilePath;
                            }
                            else if (this.missionBlueprint != null) {
                                exportFilePath = Path.Combine(modDirectory, @"missions\", this.missionBlueprint.uid + ".xml");
                                this.missionBlueprint.loadedFromFilePath = exportFilePath;
                            }
                        }
                    }
                    else
                    {
                        if (this.contractBlueprint != null)
                        {
                            exportFilePath = this.contractBlueprint.loadedFromFilePath;
                        }
                        else if (this.missionBlueprint != null)
                        {
                            exportFilePath = this.missionBlueprint.loadedFromFilePath;
                        }
                    }
                    // write
                    if(exportFilePath != String.Empty && Directory.CreateDirectory(Path.GetDirectoryName(exportFilePath)) != null)
                    {
                        Console.WriteLine($"[CM] exporting to '{exportFilePath}'");
                        if (this.contractBlueprint != null)
                        {
                            this.contractBlueprint.WriteToFile(exportFilePath);
                            this.contractBlueprint.loadedFromFilePath = exportFilePath;
                            this.contractBlueprint.isEditable = false;
                        }
                        else if (this.missionBlueprint != null)
                        {
                            this.missionBlueprint.WriteToFile(exportFilePath);
                            this.missionBlueprint.loadedFromFilePath = exportFilePath;
                            this.missionBlueprint.isEditable = false;
                        }
                    }
                    this.drawPopup = false;
                    ImGui.CloseCurrentPopup();
                }
                ImGui.EndPopup();
            }
        }
    }
}

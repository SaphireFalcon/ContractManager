using Brutal.ImGuiApi;
using ContractManager.ContractBlueprint;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContractManager.GUI
{
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

        internal void DrawPopup()
        {
            string popupTitle = String.Format("Contract: {0}##{1}", this.title, this.uid);
            bool isDrawing = false;
            ImGui.SetNextWindowSizeConstraints(
                new Brutal.Numerics.float2 { X = 300.0f, Y = 200.0f },
                new Brutal.Numerics.float2 { X = float.PositiveInfinity, Y = float.PositiveInfinity }  // no max size
            );
            if (this.popupType == PopupType.Popup)
            {
                // Using a normal window is nicer instead of a popup.
                isDrawing = ImGui.Begin(popupTitle);
            }
            else
            if (this.popupType == PopupType.Modal)
            {
                // If there is a blocking modal pop, best to put the game speed to 0.
                KSA.Universe.SetSimulationSpeed(0);
                ImGui.OpenPopup(popupTitle);
                bool refBool = this.drawPopup;
                isDrawing = ImGui.BeginPopupModal(popupTitle);
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

    internal enum PopupType {
        Popup,
        Modal
    }
}

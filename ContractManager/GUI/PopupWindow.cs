using Brutal.ImGuiApi;
using ContractManager.ContractBlueprint;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContractManager.GUI
{
    internal class PopupWindow
    {
        // Contract that triggered the popup
        internal Contract.Contract? contract { get; set;} = null;

        // Action that triggered the popup
        internal ContractBlueprint.Action? action { get; set; } = null;

        // type of popup to show.
        internal PopupType popupType { get; set; } = PopupType.Popup;

        // Flag to draw the popup or not.
        internal bool drawPopup { get; set; } = true;

        internal PopupWindow() { }

        internal void DrawPopup()
        {
            string popupTitle = String.Format("Contract: {0}##{1}_{2}", this.contract._contractBlueprint.title, this.contract.contractUID, this.action.trigger);
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
                ImGui.TextWrapped(this.action.showMessage);
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

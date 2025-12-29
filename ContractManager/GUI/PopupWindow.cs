using Brutal.ImGuiApi;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContractManager.GUI
{
    internal class PopupWindow
    {
        // Title on the bar of the popup
        internal string title { get; set; } = string.Empty;
        internal string messageToShow { get; set; } = string.Empty;
        internal PopupType popupType { get; set; } = PopupType.Popup;
        internal bool drawPopup { get; set; } = true;

        internal void DrawPopup()
        {
            string popupTitle = String.Format("Contract: {0}", title);
            bool isDrawing = false;
            ImGui.SetNextWindowSizeConstraints(
                new Brutal.Numerics.float2 { X = 300.0f, Y = 200.0f },
                new Brutal.Numerics.float2 { X = float.PositiveInfinity, Y = float.PositiveInfinity }  // no max size
            );
            if (popupType == PopupType.Popup)
            {
                // Using a normal window is nicer instead of a popup.
                isDrawing = ImGui.Begin(popupTitle);
            }
            else
            if (popupType == PopupType.Modal)
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
                    if (popupType == PopupType.Modal)
                    {
                        ImGui.CloseCurrentPopup();
                    }
                    this.drawPopup = false;
                }
                
                if (popupType == PopupType.Popup)
                {
                    ImGui.End();
                }
                if (popupType == PopupType.Modal)
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

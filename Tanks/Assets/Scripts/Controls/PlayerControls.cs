//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.3.0
//     from Assets/Scripts/PlayerControls.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @PlayerControls : IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerControls"",
    ""maps"": [
        {
            ""name"": ""Tank"",
            ""id"": ""be32b75f-c217-4cbf-9a2e-ed60b9b4360c"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""30d0c15c-54a5-49bd-8c81-2663e843b37d"",
                    ""expectedControlType"": ""Vector3"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Shoot"",
                    ""type"": ""Button"",
                    ""id"": ""b28fb2a5-9aaa-4bb8-a9f6-7d1401bb4ea7"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Aim"",
                    ""type"": ""Value"",
                    ""id"": ""ffaad4ff-afa8-4aa2-82ac-5859fa638f7d"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""NextItem"",
                    ""type"": ""Button"",
                    ""id"": ""333a64cf-9296-45b9-ad66-1e0ef6c51559"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""PreviousItem"",
                    ""type"": ""Button"",
                    ""id"": ""ec214f1d-1fa0-47c9-a1ce-dd5ec020a188"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""FocusCamera"",
                    ""type"": ""Button"",
                    ""id"": ""8f45b9c5-a57c-44ef-af9e-9bf4d3825d49"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""AutoFocusCamera"",
                    ""type"": ""Button"",
                    ""id"": ""3bcb4532-35d6-4245-b087-6db5300737b8"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Inventory"",
                    ""type"": ""Button"",
                    ""id"": ""4cee6fe4-8457-47b4-beb5-61ee7c1a83e2"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Back"",
                    ""type"": ""Button"",
                    ""id"": ""42ef6302-8c73-4be9-9e75-b7722d1dfc5e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""WASD"",
                    ""id"": ""b9a1fd91-6d10-4377-b3fd-c6687f26c9c4"",
                    ""path"": ""3DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Up"",
                    ""id"": ""56e6bf9e-ef52-4d26-b542-b09623e5548c"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": ""Clamp"",
                    ""groups"": ""Desktop"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Down"",
                    ""id"": ""0fa9e1b2-cf05-44c4-9eaf-833e6199d34a"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": ""Clamp"",
                    ""groups"": ""Desktop"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Left"",
                    ""id"": ""da8cd17f-9741-4306-9317-a5e37e842cfc"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": ""Scale(factor=0.05)"",
                    ""groups"": ""Desktop"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Right"",
                    ""id"": ""9460988a-27fc-4e2e-9587-5334a6012b95"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": ""Scale(factor=0.05)"",
                    ""groups"": ""Desktop"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Forward"",
                    ""id"": ""552023ac-6715-41e8-b84d-65e94b2cb4c5"",
                    ""path"": """",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Backward"",
                    ""id"": ""fe20cf46-5e8c-4d86-b053-d0b1b4fcc7a7"",
                    ""path"": """",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""fc3e2364-d447-458d-bbd7-4f40fffe3c13"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Desktop"",
                    ""action"": ""Shoot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""772d1191-a406-497b-a91d-f1183b07ce9d"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Desktop"",
                    ""action"": ""Aim"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d86f1650-3dc1-4321-b68e-c6ce178db934"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Desktop"",
                    ""action"": ""NextItem"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b0b2f1ee-a69b-4f97-af9b-1376f869632c"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Desktop"",
                    ""action"": ""PreviousItem"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c73fab3b-da34-439d-8dbd-2a39209bb79d"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""FocusCamera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""cc51e0fb-b2f6-4815-a8a1-95efb49563c8"",
                    ""path"": ""<Keyboard>/f"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Desktop"",
                    ""action"": ""AutoFocusCamera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e4baf8ef-1728-457c-8411-2c03a970e546"",
                    ""path"": ""<Keyboard>/i"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Desktop"",
                    ""action"": ""Inventory"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""37949de0-c664-43a2-b413-b169f338e703"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Desktop"",
                    ""action"": ""Back"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Desktop"",
            ""bindingGroup"": ""Desktop"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // Tank
        m_Tank = asset.FindActionMap("Tank", throwIfNotFound: true);
        m_Tank_Move = m_Tank.FindAction("Move", throwIfNotFound: true);
        m_Tank_Shoot = m_Tank.FindAction("Shoot", throwIfNotFound: true);
        m_Tank_Aim = m_Tank.FindAction("Aim", throwIfNotFound: true);
        m_Tank_NextItem = m_Tank.FindAction("NextItem", throwIfNotFound: true);
        m_Tank_PreviousItem = m_Tank.FindAction("PreviousItem", throwIfNotFound: true);
        m_Tank_FocusCamera = m_Tank.FindAction("FocusCamera", throwIfNotFound: true);
        m_Tank_AutoFocusCamera = m_Tank.FindAction("AutoFocusCamera", throwIfNotFound: true);
        m_Tank_Inventory = m_Tank.FindAction("Inventory", throwIfNotFound: true);
        m_Tank_Back = m_Tank.FindAction("Back", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }
    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }
    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Tank
    private readonly InputActionMap m_Tank;
    private ITankActions m_TankActionsCallbackInterface;
    private readonly InputAction m_Tank_Move;
    private readonly InputAction m_Tank_Shoot;
    private readonly InputAction m_Tank_Aim;
    private readonly InputAction m_Tank_NextItem;
    private readonly InputAction m_Tank_PreviousItem;
    private readonly InputAction m_Tank_FocusCamera;
    private readonly InputAction m_Tank_AutoFocusCamera;
    private readonly InputAction m_Tank_Inventory;
    private readonly InputAction m_Tank_Back;
    public struct TankActions
    {
        private @PlayerControls m_Wrapper;
        public TankActions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_Tank_Move;
        public InputAction @Shoot => m_Wrapper.m_Tank_Shoot;
        public InputAction @Aim => m_Wrapper.m_Tank_Aim;
        public InputAction @NextItem => m_Wrapper.m_Tank_NextItem;
        public InputAction @PreviousItem => m_Wrapper.m_Tank_PreviousItem;
        public InputAction @FocusCamera => m_Wrapper.m_Tank_FocusCamera;
        public InputAction @AutoFocusCamera => m_Wrapper.m_Tank_AutoFocusCamera;
        public InputAction @Inventory => m_Wrapper.m_Tank_Inventory;
        public InputAction @Back => m_Wrapper.m_Tank_Back;
        public InputActionMap Get() { return m_Wrapper.m_Tank; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(TankActions set) { return set.Get(); }
        public void SetCallbacks(ITankActions instance)
        {
            if (m_Wrapper.m_TankActionsCallbackInterface != null)
            {
                @Move.started -= m_Wrapper.m_TankActionsCallbackInterface.OnMove;
                @Move.performed -= m_Wrapper.m_TankActionsCallbackInterface.OnMove;
                @Move.canceled -= m_Wrapper.m_TankActionsCallbackInterface.OnMove;
                @Shoot.started -= m_Wrapper.m_TankActionsCallbackInterface.OnShoot;
                @Shoot.performed -= m_Wrapper.m_TankActionsCallbackInterface.OnShoot;
                @Shoot.canceled -= m_Wrapper.m_TankActionsCallbackInterface.OnShoot;
                @Aim.started -= m_Wrapper.m_TankActionsCallbackInterface.OnAim;
                @Aim.performed -= m_Wrapper.m_TankActionsCallbackInterface.OnAim;
                @Aim.canceled -= m_Wrapper.m_TankActionsCallbackInterface.OnAim;
                @NextItem.started -= m_Wrapper.m_TankActionsCallbackInterface.OnNextItem;
                @NextItem.performed -= m_Wrapper.m_TankActionsCallbackInterface.OnNextItem;
                @NextItem.canceled -= m_Wrapper.m_TankActionsCallbackInterface.OnNextItem;
                @PreviousItem.started -= m_Wrapper.m_TankActionsCallbackInterface.OnPreviousItem;
                @PreviousItem.performed -= m_Wrapper.m_TankActionsCallbackInterface.OnPreviousItem;
                @PreviousItem.canceled -= m_Wrapper.m_TankActionsCallbackInterface.OnPreviousItem;
                @FocusCamera.started -= m_Wrapper.m_TankActionsCallbackInterface.OnFocusCamera;
                @FocusCamera.performed -= m_Wrapper.m_TankActionsCallbackInterface.OnFocusCamera;
                @FocusCamera.canceled -= m_Wrapper.m_TankActionsCallbackInterface.OnFocusCamera;
                @AutoFocusCamera.started -= m_Wrapper.m_TankActionsCallbackInterface.OnAutoFocusCamera;
                @AutoFocusCamera.performed -= m_Wrapper.m_TankActionsCallbackInterface.OnAutoFocusCamera;
                @AutoFocusCamera.canceled -= m_Wrapper.m_TankActionsCallbackInterface.OnAutoFocusCamera;
                @Inventory.started -= m_Wrapper.m_TankActionsCallbackInterface.OnInventory;
                @Inventory.performed -= m_Wrapper.m_TankActionsCallbackInterface.OnInventory;
                @Inventory.canceled -= m_Wrapper.m_TankActionsCallbackInterface.OnInventory;
                @Back.started -= m_Wrapper.m_TankActionsCallbackInterface.OnBack;
                @Back.performed -= m_Wrapper.m_TankActionsCallbackInterface.OnBack;
                @Back.canceled -= m_Wrapper.m_TankActionsCallbackInterface.OnBack;
            }
            m_Wrapper.m_TankActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
                @Shoot.started += instance.OnShoot;
                @Shoot.performed += instance.OnShoot;
                @Shoot.canceled += instance.OnShoot;
                @Aim.started += instance.OnAim;
                @Aim.performed += instance.OnAim;
                @Aim.canceled += instance.OnAim;
                @NextItem.started += instance.OnNextItem;
                @NextItem.performed += instance.OnNextItem;
                @NextItem.canceled += instance.OnNextItem;
                @PreviousItem.started += instance.OnPreviousItem;
                @PreviousItem.performed += instance.OnPreviousItem;
                @PreviousItem.canceled += instance.OnPreviousItem;
                @FocusCamera.started += instance.OnFocusCamera;
                @FocusCamera.performed += instance.OnFocusCamera;
                @FocusCamera.canceled += instance.OnFocusCamera;
                @AutoFocusCamera.started += instance.OnAutoFocusCamera;
                @AutoFocusCamera.performed += instance.OnAutoFocusCamera;
                @AutoFocusCamera.canceled += instance.OnAutoFocusCamera;
                @Inventory.started += instance.OnInventory;
                @Inventory.performed += instance.OnInventory;
                @Inventory.canceled += instance.OnInventory;
                @Back.started += instance.OnBack;
                @Back.performed += instance.OnBack;
                @Back.canceled += instance.OnBack;
            }
        }
    }
    public TankActions @Tank => new TankActions(this);
    private int m_DesktopSchemeIndex = -1;
    public InputControlScheme DesktopScheme
    {
        get
        {
            if (m_DesktopSchemeIndex == -1) m_DesktopSchemeIndex = asset.FindControlSchemeIndex("Desktop");
            return asset.controlSchemes[m_DesktopSchemeIndex];
        }
    }
    public interface ITankActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnShoot(InputAction.CallbackContext context);
        void OnAim(InputAction.CallbackContext context);
        void OnNextItem(InputAction.CallbackContext context);
        void OnPreviousItem(InputAction.CallbackContext context);
        void OnFocusCamera(InputAction.CallbackContext context);
        void OnAutoFocusCamera(InputAction.CallbackContext context);
        void OnInventory(InputAction.CallbackContext context);
        void OnBack(InputAction.CallbackContext context);
    }
}

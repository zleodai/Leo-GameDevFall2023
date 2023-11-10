//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.7.0
//     from Assets/InputActions/PlayerInput.inputactions
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

public partial class @PlayerInput: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerInput()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerInput"",
    ""maps"": [
        {
            ""name"": ""PlayerMovement"",
            ""id"": ""aa878533-9ab7-419f-b1dd-959f48c77caa"",
            ""actions"": [
                {
                    ""name"": ""Space"",
                    ""type"": ""Button"",
                    ""id"": ""ffe0a944-81da-411e-9ed0-638bdb3882fa"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Look"",
                    ""type"": ""Value"",
                    ""id"": ""55f990ba-149f-414f-aadf-c1146cdd3e82"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Shift"",
                    ""type"": ""Button"",
                    ""id"": ""8038fb1c-4c0a-46bd-b463-c6177e3a3b18"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""WASD"",
                    ""type"": ""Value"",
                    ""id"": ""8beaa69c-904c-470b-bf65-9cbed70181e2"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""77067622-b8b1-4de4-a4f2-bbb816f363b4"",
                    ""path"": ""<Keyboard>/shift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Shift"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""fea2e688-d702-4eba-970c-0070d7a79108"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Space"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""63c35cc9-d13e-47cd-976c-58dfe89e03ce"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""WASD"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""5a9c4707-8726-4944-87c4-ee85f6d76b0f"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""WASD"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""e7dfdc6c-7507-4d2e-ae5e-b00d2a026f3f"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""WASD"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""d73c97c6-fa57-4f76-9ab7-8d7632479ccd"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""WASD"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""7f1fdd16-3b69-455a-b793-171f05662903"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""WASD"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""d9ee214e-3cb0-40aa-b87a-9908549bdeb7"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""PlayerActions"",
            ""id"": ""af080c2c-c187-4160-ba53-5e988f3e808d"",
            ""actions"": [
                {
                    ""name"": ""E"",
                    ""type"": ""Button"",
                    ""id"": ""2ea08e56-2cf1-4d5a-818f-a2e02f6ad64c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""R"",
                    ""type"": ""Button"",
                    ""id"": ""d15880ea-98fb-402d-a320-3d60b894ddcc"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""G"",
                    ""type"": ""Button"",
                    ""id"": ""5148f62d-be33-4d0e-84ea-a66f5069c613"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""1"",
                    ""type"": ""Button"",
                    ""id"": ""2635ac7e-6885-4241-b090-71a656f58dba"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""2"",
                    ""type"": ""Button"",
                    ""id"": ""b34f2688-303d-424c-b555-6dd37393ba1e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""3"",
                    ""type"": ""Button"",
                    ""id"": ""75bf1d88-01d1-4c21-b850-101125689884"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""LeftClick"",
                    ""type"": ""Button"",
                    ""id"": ""8af22bc1-9953-49cc-aa0c-f996ac05454c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""RightClick"",
                    ""type"": ""Button"",
                    ""id"": ""594b044b-6cdd-4ac8-9082-15b7f998afe3"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""f0c1e388-2de9-487a-ad94-76ba58b9ab92"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""E"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""68fc6bae-83e6-42f2-be27-a2dadfb9cb46"",
                    ""path"": ""<Keyboard>/1"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""1"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""dab1f16e-72ac-4c34-b173-87c275b48629"",
                    ""path"": ""<Keyboard>/2"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""2"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""cf529716-d330-4123-a23d-9066ad0bc2f9"",
                    ""path"": ""<Keyboard>/3"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""3"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f7ea659d-1a3a-4592-870d-9ea14e02c498"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LeftClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1a25107d-ea24-4ff7-9264-87f3e3a3a1cb"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""RightClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9dd4fb65-3c3d-4a10-9934-268b72edeb68"",
                    ""path"": ""<Keyboard>/r"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""R"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ef89212c-0efa-48ed-838f-903f08c90ccc"",
                    ""path"": ""<Keyboard>/g"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""G"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""UI"",
            ""id"": ""5382ebcf-d1e3-4270-b1c2-29e7a0a02eb2"",
            ""actions"": [
                {
                    ""name"": ""Esc"",
                    ""type"": ""Button"",
                    ""id"": ""cdfb7849-0e3b-41b1-9959-2cc00e249dcb"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""5ace3c71-2079-462a-a768-77cdbe8d1fe7"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Esc"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // PlayerMovement
        m_PlayerMovement = asset.FindActionMap("PlayerMovement", throwIfNotFound: true);
        m_PlayerMovement_Space = m_PlayerMovement.FindAction("Space", throwIfNotFound: true);
        m_PlayerMovement_Look = m_PlayerMovement.FindAction("Look", throwIfNotFound: true);
        m_PlayerMovement_Shift = m_PlayerMovement.FindAction("Shift", throwIfNotFound: true);
        m_PlayerMovement_WASD = m_PlayerMovement.FindAction("WASD", throwIfNotFound: true);
        // PlayerActions
        m_PlayerActions = asset.FindActionMap("PlayerActions", throwIfNotFound: true);
        m_PlayerActions_E = m_PlayerActions.FindAction("E", throwIfNotFound: true);
        m_PlayerActions_R = m_PlayerActions.FindAction("R", throwIfNotFound: true);
        m_PlayerActions_G = m_PlayerActions.FindAction("G", throwIfNotFound: true);
        m_PlayerActions__1 = m_PlayerActions.FindAction("1", throwIfNotFound: true);
        m_PlayerActions__2 = m_PlayerActions.FindAction("2", throwIfNotFound: true);
        m_PlayerActions__3 = m_PlayerActions.FindAction("3", throwIfNotFound: true);
        m_PlayerActions_LeftClick = m_PlayerActions.FindAction("LeftClick", throwIfNotFound: true);
        m_PlayerActions_RightClick = m_PlayerActions.FindAction("RightClick", throwIfNotFound: true);
        // UI
        m_UI = asset.FindActionMap("UI", throwIfNotFound: true);
        m_UI_Esc = m_UI.FindAction("Esc", throwIfNotFound: true);
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

    // PlayerMovement
    private readonly InputActionMap m_PlayerMovement;
    private List<IPlayerMovementActions> m_PlayerMovementActionsCallbackInterfaces = new List<IPlayerMovementActions>();
    private readonly InputAction m_PlayerMovement_Space;
    private readonly InputAction m_PlayerMovement_Look;
    private readonly InputAction m_PlayerMovement_Shift;
    private readonly InputAction m_PlayerMovement_WASD;
    public struct PlayerMovementActions
    {
        private @PlayerInput m_Wrapper;
        public PlayerMovementActions(@PlayerInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @Space => m_Wrapper.m_PlayerMovement_Space;
        public InputAction @Look => m_Wrapper.m_PlayerMovement_Look;
        public InputAction @Shift => m_Wrapper.m_PlayerMovement_Shift;
        public InputAction @WASD => m_Wrapper.m_PlayerMovement_WASD;
        public InputActionMap Get() { return m_Wrapper.m_PlayerMovement; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerMovementActions set) { return set.Get(); }
        public void AddCallbacks(IPlayerMovementActions instance)
        {
            if (instance == null || m_Wrapper.m_PlayerMovementActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_PlayerMovementActionsCallbackInterfaces.Add(instance);
            @Space.started += instance.OnSpace;
            @Space.performed += instance.OnSpace;
            @Space.canceled += instance.OnSpace;
            @Look.started += instance.OnLook;
            @Look.performed += instance.OnLook;
            @Look.canceled += instance.OnLook;
            @Shift.started += instance.OnShift;
            @Shift.performed += instance.OnShift;
            @Shift.canceled += instance.OnShift;
            @WASD.started += instance.OnWASD;
            @WASD.performed += instance.OnWASD;
            @WASD.canceled += instance.OnWASD;
        }

        private void UnregisterCallbacks(IPlayerMovementActions instance)
        {
            @Space.started -= instance.OnSpace;
            @Space.performed -= instance.OnSpace;
            @Space.canceled -= instance.OnSpace;
            @Look.started -= instance.OnLook;
            @Look.performed -= instance.OnLook;
            @Look.canceled -= instance.OnLook;
            @Shift.started -= instance.OnShift;
            @Shift.performed -= instance.OnShift;
            @Shift.canceled -= instance.OnShift;
            @WASD.started -= instance.OnWASD;
            @WASD.performed -= instance.OnWASD;
            @WASD.canceled -= instance.OnWASD;
        }

        public void RemoveCallbacks(IPlayerMovementActions instance)
        {
            if (m_Wrapper.m_PlayerMovementActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IPlayerMovementActions instance)
        {
            foreach (var item in m_Wrapper.m_PlayerMovementActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_PlayerMovementActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public PlayerMovementActions @PlayerMovement => new PlayerMovementActions(this);

    // PlayerActions
    private readonly InputActionMap m_PlayerActions;
    private List<IPlayerActionsActions> m_PlayerActionsActionsCallbackInterfaces = new List<IPlayerActionsActions>();
    private readonly InputAction m_PlayerActions_E;
    private readonly InputAction m_PlayerActions_R;
    private readonly InputAction m_PlayerActions_G;
    private readonly InputAction m_PlayerActions__1;
    private readonly InputAction m_PlayerActions__2;
    private readonly InputAction m_PlayerActions__3;
    private readonly InputAction m_PlayerActions_LeftClick;
    private readonly InputAction m_PlayerActions_RightClick;
    public struct PlayerActionsActions
    {
        private @PlayerInput m_Wrapper;
        public PlayerActionsActions(@PlayerInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @E => m_Wrapper.m_PlayerActions_E;
        public InputAction @R => m_Wrapper.m_PlayerActions_R;
        public InputAction @G => m_Wrapper.m_PlayerActions_G;
        public InputAction @_1 => m_Wrapper.m_PlayerActions__1;
        public InputAction @_2 => m_Wrapper.m_PlayerActions__2;
        public InputAction @_3 => m_Wrapper.m_PlayerActions__3;
        public InputAction @LeftClick => m_Wrapper.m_PlayerActions_LeftClick;
        public InputAction @RightClick => m_Wrapper.m_PlayerActions_RightClick;
        public InputActionMap Get() { return m_Wrapper.m_PlayerActions; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActionsActions set) { return set.Get(); }
        public void AddCallbacks(IPlayerActionsActions instance)
        {
            if (instance == null || m_Wrapper.m_PlayerActionsActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_PlayerActionsActionsCallbackInterfaces.Add(instance);
            @E.started += instance.OnE;
            @E.performed += instance.OnE;
            @E.canceled += instance.OnE;
            @R.started += instance.OnR;
            @R.performed += instance.OnR;
            @R.canceled += instance.OnR;
            @G.started += instance.OnG;
            @G.performed += instance.OnG;
            @G.canceled += instance.OnG;
            @_1.started += instance.On_1;
            @_1.performed += instance.On_1;
            @_1.canceled += instance.On_1;
            @_2.started += instance.On_2;
            @_2.performed += instance.On_2;
            @_2.canceled += instance.On_2;
            @_3.started += instance.On_3;
            @_3.performed += instance.On_3;
            @_3.canceled += instance.On_3;
            @LeftClick.started += instance.OnLeftClick;
            @LeftClick.performed += instance.OnLeftClick;
            @LeftClick.canceled += instance.OnLeftClick;
            @RightClick.started += instance.OnRightClick;
            @RightClick.performed += instance.OnRightClick;
            @RightClick.canceled += instance.OnRightClick;
        }

        private void UnregisterCallbacks(IPlayerActionsActions instance)
        {
            @E.started -= instance.OnE;
            @E.performed -= instance.OnE;
            @E.canceled -= instance.OnE;
            @R.started -= instance.OnR;
            @R.performed -= instance.OnR;
            @R.canceled -= instance.OnR;
            @G.started -= instance.OnG;
            @G.performed -= instance.OnG;
            @G.canceled -= instance.OnG;
            @_1.started -= instance.On_1;
            @_1.performed -= instance.On_1;
            @_1.canceled -= instance.On_1;
            @_2.started -= instance.On_2;
            @_2.performed -= instance.On_2;
            @_2.canceled -= instance.On_2;
            @_3.started -= instance.On_3;
            @_3.performed -= instance.On_3;
            @_3.canceled -= instance.On_3;
            @LeftClick.started -= instance.OnLeftClick;
            @LeftClick.performed -= instance.OnLeftClick;
            @LeftClick.canceled -= instance.OnLeftClick;
            @RightClick.started -= instance.OnRightClick;
            @RightClick.performed -= instance.OnRightClick;
            @RightClick.canceled -= instance.OnRightClick;
        }

        public void RemoveCallbacks(IPlayerActionsActions instance)
        {
            if (m_Wrapper.m_PlayerActionsActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IPlayerActionsActions instance)
        {
            foreach (var item in m_Wrapper.m_PlayerActionsActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_PlayerActionsActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public PlayerActionsActions @PlayerActions => new PlayerActionsActions(this);

    // UI
    private readonly InputActionMap m_UI;
    private List<IUIActions> m_UIActionsCallbackInterfaces = new List<IUIActions>();
    private readonly InputAction m_UI_Esc;
    public struct UIActions
    {
        private @PlayerInput m_Wrapper;
        public UIActions(@PlayerInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @Esc => m_Wrapper.m_UI_Esc;
        public InputActionMap Get() { return m_Wrapper.m_UI; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(UIActions set) { return set.Get(); }
        public void AddCallbacks(IUIActions instance)
        {
            if (instance == null || m_Wrapper.m_UIActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_UIActionsCallbackInterfaces.Add(instance);
            @Esc.started += instance.OnEsc;
            @Esc.performed += instance.OnEsc;
            @Esc.canceled += instance.OnEsc;
        }

        private void UnregisterCallbacks(IUIActions instance)
        {
            @Esc.started -= instance.OnEsc;
            @Esc.performed -= instance.OnEsc;
            @Esc.canceled -= instance.OnEsc;
        }

        public void RemoveCallbacks(IUIActions instance)
        {
            if (m_Wrapper.m_UIActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IUIActions instance)
        {
            foreach (var item in m_Wrapper.m_UIActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_UIActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public UIActions @UI => new UIActions(this);
    public interface IPlayerMovementActions
    {
        void OnSpace(InputAction.CallbackContext context);
        void OnLook(InputAction.CallbackContext context);
        void OnShift(InputAction.CallbackContext context);
        void OnWASD(InputAction.CallbackContext context);
    }
    public interface IPlayerActionsActions
    {
        void OnE(InputAction.CallbackContext context);
        void OnR(InputAction.CallbackContext context);
        void OnG(InputAction.CallbackContext context);
        void On_1(InputAction.CallbackContext context);
        void On_2(InputAction.CallbackContext context);
        void On_3(InputAction.CallbackContext context);
        void OnLeftClick(InputAction.CallbackContext context);
        void OnRightClick(InputAction.CallbackContext context);
    }
    public interface IUIActions
    {
        void OnEsc(InputAction.CallbackContext context);
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGenerator.ComponentsGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
namespace Entitas {
    public partial class Entity {
        public PropertyComponent property { get { return (PropertyComponent)GetComponent(ComponentIds.Property); } }

        public bool hasProperty { get { return HasComponent(ComponentIds.Property); } }

        public Entity AddProperty(string newValue) {
            var component = CreateComponent<PropertyComponent>(ComponentIds.Property);
            component.value = newValue;
            return AddComponent(ComponentIds.Property, component);
        }

        public Entity ReplaceProperty(string newValue) {
            var component = CreateComponent<PropertyComponent>(ComponentIds.Property);
            component.value = newValue;
            ReplaceComponent(ComponentIds.Property, component);
            return this;
        }

        public Entity RemoveProperty() {
            return RemoveComponent(ComponentIds.Property);
        }
    }

    public partial class Matcher {
        static IMatcher _matcherProperty;

        public static IMatcher Property {
            get {
                if (_matcherProperty == null) {
                    var matcher = (Matcher)Matcher.AllOf(ComponentIds.Property);
                    matcher.componentNames = ComponentIds.componentNames;
                    _matcherProperty = matcher;
                }

                return _matcherProperty;
            }
        }
    }
}

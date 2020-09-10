namespace BehaviourTree {
    public class Dynamic : Node {
        public Node node;

        public Dynamic(Node node) {
            this.node = node;
        }

        public void SetNode(Node node) {
            this.node = node;
        }

        public override State Evaluate() {
            return node.Evaluate();
        }
        
        public override string ToString() {
            return "Parallel Node";
        }
    }

}
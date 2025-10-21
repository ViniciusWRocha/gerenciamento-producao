
const sidebar = document.getElementById("sidebar");
const toggleButton = document.getElementById("toggle-sidebar");
const itens = document.getElementById("itensMenu");


toggleButton.addEventListener("click", () => {
	// Apenas alterna a classe no elemento principal
	sidebar.classList.toggle("collapsed");

});

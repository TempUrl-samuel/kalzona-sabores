function tema() {
    let tema = document.body
    tema.classList.toggle("dark-mode")
    let btn = document.getElementById("botao-tema")
    btn.classList.remove("girar")
    void btn.offsetWidth
    btn.classList.add("girar")
}
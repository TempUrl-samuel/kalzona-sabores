//Para qualquer pagina
function tema() {
    let tema = document.body
    tema.classList.toggle("dark-mode")

    // Salva a escolha no navegador para lembrar nas outras páginas
    if (tema.classList.contains("dark-mode")) {
        localStorage.setItem("tema", "dark")
    } else {
        localStorage.setItem("tema", "light")
    }

    let btn = document.getElementById("botao-tema")
    btn.classList.remove("girar")
    void btn.offsetWidth
    btn.classList.add("girar")
}

// Ao abrir qualquer página, reaplica o tema que foi salvo antes
if (localStorage.getItem("tema") === "dark") {
    document.body.classList.add("dark-mode")
}

// Menu hambúrguer: abre/fecha a lista do header no celular
function menuHamburguer() {
    document.getElementById("lista-header").classList.toggle("aberto")
}

//para qualquer pagina.
//Cadastro

// Máscara de Celular: (XX) XXXXX-XXXX ou (XX) XXXX-XXXX
function mascaraTelefone() {
    var campo = document.getElementById("telefone");
    var valor = campo.value.replace(/\D/g, ""); // Remove tudo que não for número

    if (valor.length > 10) {
        // Formato para 11 dígitos (celular moderno)
        campo.value = valor.replace(/^(\d{2})(\d{5})(\d{4}).*/, "($1) $2-$3");
    } else if (valor.length > 5) {
        // Formato enquanto está digitando
        campo.value = valor.replace(/^(\d{2})(\d{4})(\d{0,4}).*/, "($1) $2-$3");
    } else if (valor.length > 2) {
        campo.value = valor.replace(/^(\d{2})(\d{0,5})/, "($1) $2");
    } else {
        campo.value = valor;
    }
}

// Máscara de CEP: 00000-000
function mascaraCEP() {
    var campo = document.getElementById("cep");
    var valor = campo.value.replace(/\D/g, ""); // Remove tudo que não for número

    campo.value = valor.replace(/^(\d{5})(\d{1,3}).*/, "$1-$2");
}

// Máscara de CPF: 000.000.000-00
function mascaraCPF() {
    var campo = document.getElementById("cpf");
    var valor = campo.value.replace(/\D/g, ""); // Remove tudo que não for número

    campo.value = valor
        .replace(/^(\d{3})(\d)/, "$1.$2")
        .replace(/^(\d{3})\.(\d{3})(\d)/, "$1.$2.$3")
        .replace(/\.(\d{3})(\d)/, ".$1-$2")
        .slice(0, 14); // Garante que não passe de 14 caracteres
}

// Mantendo sua função de limitação de número se necessário
function limitacao() {
    var numero = document.getElementById("numero");
    numero.value = numero.value.replace(/\D/g, ""); // Permite só números no campo 'Número'
}
//cadastro.
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
//PERFIL (salvo no localStorage do navegador)

var CHAVE_PERFIL = "perfil"         // dados do formulário
var CHAVE_FOTO = "fotoPerfil"       // foto em formato de texto (data URL)
var CAMPOS_PERFIL = ["nome", "email", "telefone", "cep", "rua", "numero", "complemento", "cidade", "estado"]

// O localStorage pode falhar (aba anônima, armazenamento cheio), por isso o try/catch
function lerStorage(chave) {
    try { return localStorage.getItem(chave) } catch (e) { return null }
}

function gravarStorage(chave, valor) {
    try { localStorage.setItem(chave, valor); return true } catch (e) { return false }
}

function apagarStorage(chave) {
    try { localStorage.removeItem(chave) } catch (e) { }
}

// Coloca a foto salva dentro de um círculo (o botão do header ou a prévia do perfil)
// Sem foto salva, volta a mostrar o ícone de pessoa
function aplicarFoto(elemento) {
    if (!elemento) return
    var foto = lerStorage(CHAVE_FOTO)
    var img = elemento.querySelector("img")
    var icone = elemento.querySelector("svg")

    if (foto) {
        if (!img) {
            img = document.createElement("img")
            img.alt = ""
            elemento.appendChild(img)
        }
        img.src = foto
        if (icone) icone.style.display = "none"
    } else {
        if (img) img.remove()
        if (icone) icone.style.display = ""
    }

    var remover = document.getElementById("remover-foto")
    if (remover) remover.hidden = !foto
}

function atualizarFotos() {
    aplicarFoto(document.getElementById("botao-perfil"))
    aplicarFoto(document.getElementById("foto-preview"))
}

function avisoPerfil(texto) {
    var status = document.getElementById("status-perfil")
    if (status) status.textContent = texto
}

// Preenche o formulário do perfil com o que já foi salvo
function carregarPerfil() {
    if (!document.getElementById("form-perfil")) return
    var dados = {}
    try { dados = JSON.parse(lerStorage(CHAVE_PERFIL)) || {} } catch (e) { }

    CAMPOS_PERFIL.forEach(function (id) {
        var campo = document.getElementById(id)
        if (campo && dados[id] !== undefined) campo.value = dados[id]
    })
}

// Botão "Salvar alterações"
function salvarPerfil() {
    var dados = {}
    CAMPOS_PERFIL.forEach(function (id) {
        var campo = document.getElementById(id)
        if (campo) dados[id] = campo.value.trim()
    })

    if (gravarStorage(CHAVE_PERFIL, JSON.stringify(dados))) {
        avisoPerfil("Alterações salvas!")
    } else {
        avisoPerfil("Não foi possível salvar neste navegador.")
    }
}

// Botão "Alterar foto": reduz a imagem para 256x256 (recorte quadrado no centro)
// para caber no localStorage, salva e já atualiza o botão do header
function trocarFoto(input) {
    var arquivo = input.files[0]
    input.value = "" // permite escolher a mesma foto de novo depois
    if (!arquivo) return

    if (!arquivo.type.startsWith("image/")) {
        avisoPerfil("Escolha um arquivo de imagem.")
        return
    }

    var leitor = new FileReader()
    leitor.onload = function () {
        var imagem = new Image()
        imagem.onload = function () {
            var tamanho = 256
            var lado = Math.min(imagem.width, imagem.height)
            var canvas = document.createElement("canvas")
            canvas.width = tamanho
            canvas.height = tamanho
            var ctx = canvas.getContext("2d")
            ctx.fillStyle = "white" // fundo branco para PNG transparente não ficar preto
            ctx.fillRect(0, 0, tamanho, tamanho)
            ctx.drawImage(
                imagem,
                (imagem.width - lado) / 2, (imagem.height - lado) / 2, lado, lado,
                0, 0, tamanho, tamanho
            )

            if (gravarStorage(CHAVE_FOTO, canvas.toDataURL("image/jpeg", 0.85))) {
                atualizarFotos()
                avisoPerfil("Foto atualizada!")
            } else {
                avisoPerfil("Não foi possível salvar a foto neste navegador.")
            }
        }
        imagem.onerror = function () { avisoPerfil("Não foi possível abrir essa imagem.") }
        imagem.src = leitor.result
    }
    leitor.readAsDataURL(arquivo)
}

// Botão "Remover foto"
function removerFoto() {
    apagarStorage(CHAVE_FOTO)
    atualizarFotos()
    avisoPerfil("Foto removida.")
}

// Ao abrir qualquer página: mostra a foto no botão do header (e preenche o perfil, se for a página de perfil)
atualizarFotos()
carregarPerfil()

// Se a foto mudar em outra aba aberta do site, atualiza esta também
window.addEventListener("storage", atualizarFotos)

//perfil.

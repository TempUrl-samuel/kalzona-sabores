//Para qualquer pagina
function tema() {
    let tema = document.body
    tema.classList.toggle("dark-mode")

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

if (localStorage.getItem("tema") === "dark") {
    document.body.classList.add("dark-mode")
}

function menuHamburguer() {
    document.getElementById("lista-header").classList.toggle("aberto")
}


//Cadastro


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


function mascaraCEP() {
    var campo = document.getElementById("cep");
    var valor = campo.value.replace(/\D/g, "");

    campo.value = valor.replace(/^(\d{5})(\d{1,3}).*/, "$1-$2");
}


function mascaraCPF() {
    var campo = document.getElementById("cpf");
    var valor = campo.value.replace(/\D/g, "");

    campo.value = valor
        .replace(/^(\d{3})(\d)/, "$1.$2")
        .replace(/^(\d{3})\.(\d{3})(\d)/, "$1.$2.$3")
        .replace(/\.(\d{3})(\d)/, ".$1-$2")
        .slice(0, 14);
}


function limitacao() {
    var numero = document.getElementById("numero");
    numero.value = numero.value.replace(/\D/g, ""); 
}
//cadastro.
//PERFIL (salvo no localStorage do navegador)

var CHAVE_PERFIL = "perfil"
var CHAVE_FOTO = "fotoPerfil"
var CAMPOS_PERFIL = ["nome", "email", "telefone", "cep", "rua", "numero", "complemento", "cidade", "estado"]

function lerStorage(chave) {
    try { return localStorage.getItem(chave) } catch (e) { return null }
}

function gravarStorage(chave, valor) {
    try { localStorage.setItem(chave, valor); return true } catch (e) { return false }
}

function apagarStorage(chave) {
    try { localStorage.removeItem(chave) } catch (e) { }
}


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


function carregarPerfil() {
    if (!document.getElementById("form-perfil")) return
    var dados = {}
    try { dados = JSON.parse(lerStorage(CHAVE_PERFIL)) || {} } catch (e) { }

    CAMPOS_PERFIL.forEach(function (id) {
        var campo = document.getElementById(id)
        if (campo && dados[id] !== undefined) campo.value = dados[id]
    })
}


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

// Botão "Alterar foto"
function trocarFoto(input) {
    var arquivo = input.files[0]
    input.value = ""
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
            ctx.fillStyle = "white"
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

function removerFoto() {
    apagarStorage(CHAVE_FOTO)
    atualizarFotos()
    avisoPerfil("Foto removida.")
}

atualizarFotos()
carregarPerfil()

window.addEventListener("storage", atualizarFotos)

//FERRAMENTA DE BUSCA
var ITENS_BUSCA = [
    "Calzone de calabresa",
    "Calzone de frango com catupiry",
    "Calzone vegetariano",
    "Esfiha de calabresa",
    "Esfiha de frango com catupiry",
    "Esfiha vegetariana",
    "Calzone de nutella com morango",
    "Calzone de doce de leite com banana",
    "Esfiha de nutella com morango",
    "Esfiha de doce de leite com banana",
    "Breadsticks",
    "Stromboli",
    "Molho de chocolate para breadsticks"
]

var DESTINO_BUSCA = "index.html#sabores-e-alternativas"

function normalizar(texto) {
    return texto.toLowerCase().normalize("NFD").replace(/[̀-ͯ]/g, "").trim()
}

// Cria a caixa de busca uma única vez (um <dialog>, que fica por cima de tudo)
function criarBusca() {
    var caixa = document.createElement("dialog")
    caixa.id = "caixa-busca"
    caixa.innerHTML =
        '<div class="busca-conteudo">' +
        '  <div class="busca-topo">' +
        '    <input type="search" id="campo-busca" placeholder="O que você procura?" autocomplete="off" aria-label="Pesquisar no cardápio">' +
        '    <button type="button" id="fechar-busca" aria-label="Fechar pesquisa">&times;</button>' +
        '  </div>' +
        '  <ul id="lista-itens"></ul>' +
        '  <p id="busca-vazia" hidden>Nenhum item encontrado.</p>' +
        '</div>'
    document.body.appendChild(caixa)

    var campo = document.getElementById("campo-busca")
    campo.addEventListener("input", filtrarItens)

    campo.addEventListener("keydown", function (evento) {
        if (evento.key === "Escape") {
            evento.preventDefault()
            fecharBusca()
        }
    })
    document.getElementById("fechar-busca").addEventListener("click", fecharBusca)

    caixa.addEventListener("click", function (evento) {
        if (evento.target === caixa) fecharBusca()
    })
    return caixa
}

function abrirBusca() {
    var caixa = document.getElementById("caixa-busca") || criarBusca()
    var campo = document.getElementById("campo-busca")
    campo.value = ""
    filtrarItens()
    caixa.showModal() 
    campo.focus()
}

function fecharBusca() {
    var caixa = document.getElementById("caixa-busca")
    if (caixa && caixa.open) caixa.close()
}

function filtrarItens() {
    var termo = normalizar(document.getElementById("campo-busca").value)
    var lista = document.getElementById("lista-itens")
    var vazio = document.getElementById("busca-vazia")
    lista.innerHTML = ""

    if (termo === "") {
        vazio.hidden = true
        return
    }

    var encontrados = ITENS_BUSCA.filter(function (item) {
        return normalizar(item).includes(termo)
    })

    encontrados.forEach(function (item) {
        var li = document.createElement("li")
        var link = document.createElement("a")
        link.href = DESTINO_BUSCA
        link.textContent = item
        link.addEventListener("click", fecharBusca)
        li.appendChild(link)
        lista.appendChild(li)
    })

    vazio.hidden = encontrados.length > 0
}
//ferramenta de busca.

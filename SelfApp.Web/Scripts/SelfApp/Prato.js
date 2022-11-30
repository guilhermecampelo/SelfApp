function set_dados_form(dados) {
    $('#id_cadastro').val(dados.Id);
    $('#txt_nome').val(dados.Nome);
    $('#txt_descricao').val(dados.Nome);
    $('#txt_preco_venda').val(dados.PrecoVenda);
    $('#cbx_ativo').prop('checked', dados.Ativo);
    //$('#txt_imagem').val(dados.Imagem);
}

function set_focus_form() {
    var alterando = (parseInt($('#id_cadastro').val()) > 0);

    $('#txt_nome').focus();
}

function get_dados_inclusao() {
    return {
        Id: 0,
        Nome: '',
        Descricao: '',
        PrecoVenda: 0,
        Ativo: true,
        Imagem: '',
    };
}

function get_dados_form() {
    var form = new FormData();
    form.append('Id', $('#id_cadastro').val());
    form.append('Nome', $('#txt_nome').val());
    form.append('Descricao', $('#txt_descricao').val());
    form.append('PrecoVenda', $('#txt_preco_venda').val());
    form.append('Ativo', $('#cbx_ativo').prop('checked'));
    form.append('Imagem', $('#txt_imagem').prop('files')[0]);
    form.append('__RequestVerificationToken', $('[name=__RequestVerificationToken]').val());
    return form;
}

function preencher_linha_grid(param, linha) {
    linha
        .eq(0).html(param.Nome).end()
        .eq(1).html(param.PrecoVenda).end()
        .eq(2).html(param.Ativo ? 'SIM' : 'NÃO');
}

function salvar_customizado(url, param, salvar_ok, salvar_erro) {
    $.ajax({
        type: 'POST',
        processData: false,
        contentType: false,
        data: param,
        url: url,
        dataType: 'json',
        success: function (response) {
            salvar_ok(response, get_param());
        },
        error: function () {
            salvar_erro();
        }
    });
}

function get_param() {
    return {
        Id: $('#id_cadastro').val(),
        Nome: $('#txt_nome').val(),
        Descricao: $('#txt_descricao').val(),
        PrecoVenda: $('#txt_preco_venda').val(),
        Ativo: $('#cbx_ativo').prop('checked'),
        Imagem: $('#txt_imagem').prop('files')[0]
    };
}

$(document)
    .ready(function () {
        $('#txt_preco_venda').mask('#.##0,00', { reverse: true });
    })
    .on('click', '.btn-exibir-imagem', function () {
        var nome_imagem = $(this).closest('tr').attr('data-imagem'),
            nome_teste = $(this).closest('tr').attr('nome-teste'),
            modal_imagem = $('#modal_imagem'),
            template_imagem = $('#template-imagem'),
            conteudo_modal_imagem = Mustache.render(template_imagem.html(), { Imagem: nome_imagem });

        modal_imagem.html(conteudo_modal_imagem);

        bootbox.dialog({
            title: `${nome_teste}`,
            message: modal_imagem,
            className: 'dialogo'
        })
            .on('shown.bs.modal', function () {
                modal_imagem.show();
            })
            .on('hidden.bs.modal', function () {
                modal_imagem.hide().appendTo('body');
            });
    });
function showData() {
    const divPai = document.getElementById('body');
    if (divPai)
        divPai.innerHTML = '';

    $.ajax({
        url: 'https://localhost:44375/api/Dados/GetDados',
        type: 'POST',
        contentType: 'application/json',
        success: function (data) {
            if (data.success) {
                let count = 0;
                const item = ["Jogo numero:", "Total de mortes por jogo", "Total de mortes por <world>", "Total de mortes por causa"];
                const labels = Object.values(item);
                data.jogos.ret.forEach(it => {
                    const num_jogo = it.numero_jogo;
                    const total_mortes_por_jogo = it.total_mortes_por_jogo;
                    const total_mortes_por_world = it.total_mortes_por_world;
                    var causa_mortis = it.lst_causa;
                    const divFilha = document.createElement('div');
                    divFilha.classList.add('item' + count);
                    divFilha.classList.add('estilo');
                    labels.forEach(label => {
                        const labelElement = document.createElement('label');
                        if (label.includes('numero:')) {
                            labelElement.textContent = label + ' ' + num_jogo;
                        } else {
                            labelElement.textContent = label;
                        }
                        divFilha.appendChild(labelElement);

                        if (label.includes('mortes por causa')) {
                            causa_mortis.forEach(cause => {
                                const labelElem = document.createElement('label');
                                labelElem.classList.add('quad');
                                labelElem.textContent = cause.descricao_meansOfDeath + ' :' + cause.morteXCausa;
                                divFilha.appendChild(labelElem);
                            });
                        } else if (!label.includes('numero:')) {
                            const inputElement = document.createElement('input');
                            inputElement.classList.add('inp');
                            inputElement.readOnly = true;
                            if (label.includes('mortes por jogo')) {
                                inputElement.value = total_mortes_por_jogo;
                            } else if (label.includes('mortes por <world>')) {
                                inputElement.value = total_mortes_por_world;
                            }
                            divFilha.appendChild(inputElement);
                        }
                    });
                    count++;
                    divPai.appendChild(divFilha);
                });
            }

        }
    });
}

function showRanking() {
    const divPai = document.getElementById('body');
    if (divPai)
        divPai.innerHTML = '';

    $.ajax({
        url: 'https://localhost:44375/api/Dados/GetRanking',
        type: 'POST',
        contentType: 'application/json',
        success: function (data) {
            if (data.success) {
                let count = 0;
                const item = ["Jogo numero:", "Jogador:", "Pontuação"];
                const labels = Object.values(item);
                data.jogos.forEach(it => {
                    const num_jogo = it.numeroDoJogo;
                    const Jogador = it.nomeJogador;
                    const Pontucao = it.pontuacao;

                    const divFilha = document.createElement('div');
                    divFilha.classList.add('item' + count);
                    divFilha.classList.add('estilo');
                    labels.forEach(label => {
                        const labelElement = document.createElement('label');
                        if (label.includes('numero:')) {
                            labelElement.textContent = label + ' ' + num_jogo;
                        } else {
                            labelElement.textContent = label;
                        }
                        divFilha.appendChild(labelElement);

                        if (!label.includes('numero:')) {
                            const inputElement = document.createElement('input');
                            inputElement.classList.add('inp1');
                            inputElement.readOnly = true;
                            if (label.includes('Jogador')) {
                                inputElement.value = Jogador;
                            } else if (label.includes('Pontuação')) {
                                inputElement.value = Pontucao;
                            }
                            divFilha.appendChild(inputElement);
                        }
                    });
                    count++;
                    divPai.appendChild(divFilha);
                    });
                }

        }
        });
}
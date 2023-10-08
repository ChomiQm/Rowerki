import React from 'react';
import axios from 'axios';


export default class BikesAdd extends React.Component {
    state = {
        bikeName: "",
        bikeDescription: "",
        quantity: 0,
        dateOfStore: Date(),
        manuDepSeriesId: 0
    }

    handleChangeName = event => {
        this.setState({ bikeName: event.target.value });
    }
    handleChangeDesc = event => {
        this.setState({ bikeDescription: event.target.value });
    }
    handleChangeQty = event => {
        this.setState({ quantity: event.target.value });
    }
    handleChangeDate = event => {
        this.setState({ dateOfStore: event.target.value });
    }
    handleChangeManu = event => {
        this.setState({ manuDepSeriesId: event.target.value });
    }

    handleSubmit = event => {
        event.preventDefault();

        const bikeName = this.state.bikeName;
        const bikeDescription = this.state.bikeDescription;
        const quantity = this.state.quantity;
        const dateOfStore = this.state.dateOfStore;
        const manuDepSeriesId = this.state.manuDepSeriesId;

        let bodyFormData = new FormData();
        bodyFormData.append("bikeName", bikeName);
        bodyFormData.append("bikeDescription", bikeDescription);
        bodyFormData.append("quantity", quantity);
        bodyFormData.append("dateOfStore", dateOfStore);
        bodyFormData.append("manuDepSeriesId", manuDepSeriesId);

        axios({
            method: "post",
            url: "BikeDepot",
            data: bodyFormData,
            headers: { "Content-Type": "multipart/form-data" }
        })
            .then(res => {
                console.log(res);
                console.log(res.data);
            })
    }

    render() {
        return (
            <div>
                <form onSubmit={this.handleSubmit}>
                    <label>
                        Prosze zeby to dzialalo tylko:
                        <input type="text" name="bikeName" onChange={this.handleChangeName} />
                        <input type="text" name="bikeDescription" onChange={this.handleChangeDesc} />
                        <input type="number" name="quantity" onChange={this.handleChangeQty} />
                        <input type="date" name="dateOfStore" onChange={this.handleChangeDate} />
                        <input type="number" name="manuDepSeriesId" onChange={this.handleChangeManu} />
                    </label>
                    <button type="submit">Add</button>
                </form>
            </div>
        )
    }

}
import React from 'react';
import axios from 'axios';

export default class BikesDelete extends React.Component {
    state = {
        bikeId: 0
    }

    handleChange = event => {
        this.setState({ bikeId: event.target.value });
    }

    handleSubmit = event => {
        event.preventDefault();

        axios.delete(`BikeDepot/${this.state.bikeId}`)
            .then(res => {
                console.log(res);
                console.log(res.data);
                window.location.reload();
            })
    }

    render() {
        return (
            <div>
                <form onSubmit={this.handleSubmit}>
                    <label>
                        Bike ID:
                        <input type="number" name="bikeId" onChange={this.handleChange} />
                    </label>
                    <button type="submit">Delete</button>
                </form>
            </div>
        )
    }
}